using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using System.Linq;

namespace OrganismTest
{
    public class NEATControllerOrg : MonoBehaviour
    {
        const int NUM_INPUTS = 8;
        public static int NUM_OUTPUTS
        {
            get { return TestSettings.Instance.MaxMuscles + 2; }
        }

        public int Trials;
        public bool GenomeEvaluated = false;
        public float StoppingFitness;
        bool EARunning;
        string popFileSavePath, champFileSavePath, fitFileSavePath;

        OrganismExperiment experiment;
        static NeatEvolutionAlgorithm<NeatGenome> _ea;

        public GameObject Unit;
        private GameObject obj;
        private GameObject shark;
        List<Vector2> fishStartPos = new List<Vector2>();
        static List<float> AllHighestGenFit = new List<float>();

        Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
        private DateTime startTime;
        private float timeLeft;
        private float accum;
        private int frames;
        private float updateInterval = 12;

        private uint Generation;
        private double Fitness;

        public static float HighestFitness = 0;
        public int TrialDuration = 15;

        //public float TrialDuration;
        // Use this for initialization
        void Awake()
        {
            try
            {
                //shark = GameObject.FindGameObjectWithTag("Shark");
                //fishStartPos = GetVector2FromRadius(shark.transform.position, FishSettings.Instance.SpawnRadius, FishSettings.Instance.MaxFish);
            }
            catch
            {
                Debug.Log("Found no object with the tag Shark in the scene");
            }
            Utility.DebugLog = true;
            experiment = new OrganismExperiment();
            XmlDocument xmlConfig = new XmlDocument();
            TextAsset textAsset = (TextAsset)Resources.Load("experiment.config");
            xmlConfig.LoadXml(textAsset.text);
            experiment.SetController(this);

            experiment.Initialize("Organism Experiment", xmlConfig.DocumentElement, NUM_INPUTS, NUM_OUTPUTS);

            champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", TestSettings.Instance.ProjectName);
            popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", TestSettings.Instance.ProjectName);
            fitFileSavePath = Application.persistentDataPath + string.Format("/{0}.fit.txt", TestSettings.Instance.ProjectName);


            print(champFileSavePath);
        }
        /// <summary>
        /// Returns a list of vector2s in a circle around a point
        /// </summary>
        /// <param name="fromPos">Origin</param>
        /// <param name="radius">Radius from origin</param>
        /// <param name="amount">Amount of points</param>
        /// <returns></returns>
        List<Vector2> GetVector2FromRadius(Vector2 fromPos, float radius, int amount)
        {
            List<Vector2> Vector2FromRadius = new List<Vector2>();
            float radiansPerFish = (Mathf.PI * 2) / amount;
            for (int i = 0; i < amount; i++)
            {
                float angle = radiansPerFish * i;

                Vector2 newPos = fromPos;
                newPos += new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                Vector2FromRadius.Add(newPos);
            }
            return Vector2FromRadius;
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            //  evaluationStartTime += Time.deltaTime;        
            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeLeft <= 0.0)
            {
                var fps = accum / frames;
                timeLeft = updateInterval;
                accum = 0.0f;
                frames = 0;
                //print("FPS: " + fps);
                if (fps < 10 && Time.timeScale > 1)
                {
                    Time.timeScale = Time.timeScale - 1;
                    print("Lowering time scale to " + Time.timeScale);
                }
            }
        }

        public void StartEA()
        {
            Utility.DebugLog = true;
            Utility.Log("Starting experiment");
            // print("Loading: " + popFileLoadPath);
            _ea = experiment.CreateEvolutionAlgorithm(popFileSavePath);
            startTime = DateTime.Now;

            _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
            _ea.PausedEvent += new EventHandler(ea_PauseEvent);

            var evoSpeed = 75;

            //   Time.fixedDeltaTime = 0.045f;
            Time.timeScale = evoSpeed;
            _ea.StartContinue();
            EARunning = true;
        }

        void ea_UpdateEvent(object sender, EventArgs e)
        {
            Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
                _ea.CurrentGeneration, _ea.Statistics._maxFitness));

            Fitness = _ea.Statistics._maxFitness;

            if (HighestFitness < Fitness)
            {
                HighestFitness = (float)System.Math.Round(Fitness, 2);
                XmlWriterSettings _xwSettings = new XmlWriterSettings();
                _xwSettings.Indent = true;
                using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
                {
                    experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
                }
            }
            AllHighestGenFit.Add((float)System.Math.Round(Fitness, 2));

            Generation = _ea.CurrentGeneration;

            //Debug.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));
        }

        void ea_PauseEvent(object sender, EventArgs e)
        {
            Time.timeScale = 1;
            Utility.Log("Done ea'ing (and neat'ing)");

            XmlWriterSettings _xwSettings = new XmlWriterSettings();
            _xwSettings.Indent = true;
            // Save genomes to xml file.        
            DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
            if (!dirInf.Exists)
            {
                Debug.Log("Creating subdirectory");
                dirInf.Create();
            }
            using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
            {
                experiment.SavePopulation(xw, _ea.GenomeList);
            }
            // Also save the best genome
            //using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
            //{
            //    experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
            //}
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fitFileSavePath, true))
            {
                for (int i = 0; i < AllHighestGenFit.Count; i++)
                {
                    file.WriteLine("Gen: " + (i + 1) + "  Fitness: " + AllHighestGenFit[i]);
                }
            }
            //float[] fitArray = AllHighestGenFit.ToArray();
            //System.IO.File.WriteAllLines(fitFileSavePath, fitArray.Select(a => a.ToString()).ToArray());

            DateTime endTime = DateTime.Now;
            Utility.Log("Total time elapsed: " + (endTime - startTime));

            System.IO.StreamReader stream = new System.IO.StreamReader(popFileSavePath);

            EARunning = false;

        }

        public void StopEA()
        {
            if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
            {
                _ea.Stop();
            }
        }

        public void Evaluate(IBlackBox box)
        {
            int rand = UnityEngine.Random.Range(0, TestSettings.Instance.MaxFish);
            //Vector3 pos = new Vector3(fishStartPos[rand].x,fishStartPos[rand].y);
            obj = Instantiate(Unit, transform.position, Unit.transform.rotation) as GameObject;
            obj.AddComponent<Creature>();
            UnitController controller = obj.GetComponent<UnitController>();
            ControllerMap.Add(box, controller);
            controller.Activate(box);
        }

        public void StopEvaluation(IBlackBox box)
        {
            UnitController ct = ControllerMap[box];

            Destroy(ct.gameObject);
        }

        public void RunBest()
        {
            Time.timeScale = 1;

            NeatGenome genome = null;

            // Try to load the genome from the XML document. 
            try
            {
                using (XmlReader xr = XmlReader.Create(champFileSavePath))
                    genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];
            }
            catch (Exception e1)
            {
                Debug.Log(" Error loading genome from file! Loading aborted.\n" + e1.Message);
                return;
            }

            // Get a genome decoder that can convert genomes to phenomes.
            var genomeDecoder = experiment.CreateGenomeDecoder();

            // Decode the genome into a phenome (neural network).
            var phenome = genomeDecoder.Decode(genome);

            GameObject obj = Instantiate(Unit, transform.position, Unit.transform.rotation) as GameObject;
            obj.AddComponent<Creature>();
            UnitController controller = obj.GetComponent<UnitController>();

            ControllerMap.Add(phenome, controller);

            controller.Activate(phenome);
        }

        public float GetFitness(IBlackBox box)
        {
            if (ControllerMap.ContainsKey(box))
            {
                return ControllerMap[box].GetFitness();
            }
            return 0;
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(30, 20, 100, 50), "Start EA"))
            {
                Reset();
                StartEA();
            }
            if (GUI.Button(new Rect(30, 70, 100, 50), "Stop EA"))
            {
                StopEA();
            }
            if (GUI.Button(new Rect(30, 120, 100, 50), "Run best"))
            {
                Reset();
                RunBest();
            }
            if (GUI.Button(new Rect(30, 170, 100, 50), "Delete saves"))
            {
                DeleteSaves();
            }
            if (GUI.Button(new Rect(30, 220, 100, 50), "Create Graph"))
            {
               gameObject.GetComponent<GraphMaker>().CreateGraph(fitFileSavePath);
            }
            GUIStyle lStyle = GUI.skin.button;
            GUI.skin.label = lStyle;
            GUIStyle bStyle = GUI.skin.button;
            GUI.Label(new Rect(Screen.width - 100, 250, 100, 50), String.Format("TimeScale: \n {0}", Time.timeScale));
            if (GUI.Button(new Rect(Screen.width - 20, 280, 20, 20), ""))
            {
                Time.timeScale++;
            }
            if (GUI.Button(new Rect(Screen.width - 100, 280, 20, 20), ""))
            {
                Time.timeScale--;
            }
            GUI.Label(new Rect(30, Screen.height - 70, 100, 50), string.Format("Generation: {0}\nFitness: {1:0.00}", Generation, Fitness));
            GUI.Label(new Rect(30, Screen.height - 130, 100, 50), string.Format("HighestFitness:\n {0}", HighestFitness));
        }

        private void DeleteSaves()
        {
            if (File.Exists(Application.persistentDataPath + string.Format("/{0}.pop.xml", TestSettings.Instance.ProjectName)) || File.Exists(Application.persistentDataPath + string.Format("/{0}.pop.xml", TestSettings.Instance.ProjectName)) || File.Exists(fitFileSavePath))
            {
                File.Delete(Application.persistentDataPath + string.Format("/{0}.champ.xml", TestSettings.Instance.ProjectName));
                File.Delete(Application.persistentDataPath + string.Format("/{0}.pop.xml", TestSettings.Instance.ProjectName));
                File.Delete(fitFileSavePath);
            }
            else
            {
                print("The files you want to delete does not exist");
            }
        }
        private void Reset()
        {
            Camera.main.transform.rotation = Quaternion.Euler(4.2f, 0, 0);
            Camera.main.orthographicSize = 9.5f;
            gameObject.transform.position = new Vector3(0, 5, -20);
        }
    }

}