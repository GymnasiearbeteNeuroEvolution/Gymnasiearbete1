using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;

public class NEATController : MonoBehaviour {

    const int NUM_INPUTS = 4;
    const int NUM_OUTPUTS = 2;

    public int Trials;
    //public float TrialDuration = 0;
    public bool GenomeEvaluated = false;
    public float StoppingFitness;
    bool EARunning;
    string popFileSavePath, champFileSavePath;

    FishExperiment experiment;
    static NeatEvolutionAlgorithm<NeatGenome> _ea;

    public GameObject Unit;
    private GameObject obj;
    private GameObject shark;
    List<Vector2> fishStartPos = new List<Vector2>();

    Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
    private DateTime startTime;
    private float timeLeft;
    private float accum;
    private int frames;
    private float updateInterval = 12;

    private uint Generation;
    private double Fitness;

    public int TrialDuration = 15;


    //public float TrialDuration;
    // Use this for initialization
    void Start () {
        try
        {
            shark = GameObject.FindGameObjectWithTag("Shark");
            fishStartPos = GetVector2FromRadius(shark.transform.position, FishSettings.Instance.SpawnRadius, FishSettings.Instance.MaxFish);
        }
        catch
        {
            Debug.Log("Found no object with the tag Shark in the scene");
        }
        Utility.DebugLog = true;
        experiment = new FishExperiment();
        XmlDocument xmlConfig = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load("experiment.config");
        xmlConfig.LoadXml(textAsset.text);
        experiment.SetController(this);

        experiment.Initialize("Fish Experiment", xmlConfig.DocumentElement, NUM_INPUTS, NUM_OUTPUTS);

        champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", "FishExperiment");
        popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", "FishExperiment");

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
            if (fps < 10)
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

        var evoSpeed = 25;

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

        using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
        }
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
        int rand = UnityEngine.Random.Range(0, FishSettings.Instance.MaxFish);
        //Vector3 pos = new Vector3(fishStartPos[rand].x,fishStartPos[rand].y);
        obj = Instantiate(Unit, transform.position, Unit.transform.rotation) as GameObject;
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
        Time.timeScale = 3;

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
            StartEA();
        }
        if (GUI.Button(new Rect(30, 70, 100, 50), "Stop EA"))
        {
            StopEA();
        }
        if (GUI.Button(new Rect(30, 120, 100, 50), "Run best"))
        {
            RunBest();
        }
        if (GUI.Button(new Rect(30, 170, 100, 50), "Delete saves"))
        {
            DeleteSaves();
        }

        GUI.Button(new Rect(30, Screen.height - 70, 100, 50), string.Format("Generation: {0}\nFitness: {1:0.00}", Generation, Fitness));
    }

    private void DeleteSaves()
    {
        File.Delete(Application.persistentDataPath + string.Format("/{0}.champ.xml", "FishExperiment"));
        File.Delete(Application.persistentDataPath + string.Format("/{0}.pop.xml", "FishExperiment"));
    }
}
