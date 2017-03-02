using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using System.Linq;

/// <summary>
/// A creature composed of nodes and muscles;
/// creatures have an internal clock that ticks continously;
/// creatures have a fitness value
/// </summary>
class Creature : UnitController
{
    private IBlackBox box;

    private ISignalArray outputArr;

    private ISignalArray inputArr;


    //Boolean deciding whether or not it is running 
    bool IsRunning = true;

    //Timer
    float timer = 0;
    //The fitness of the this creature
    private float fitness;

    // the chance of a big mutation occuring
    const float mutationConstant = 0.06f;

    // the number of operation that should be made
    const int operationCount = 12;

    // a list of all the nodes the creature has
    public List<Node> nodes;

    // a list of all the muscles the creature has
    public List<Muscle> muscles;

    // the creatures index value in the creature list
    int ID;

    // fitness is equal to the positive distance travelled in the chosen axis
    float cFitness;

    // represents the creatures current state of existence
    bool alive;

    // the internal clock of the creature, the "heartbeat"
    float creatureTimer;

    // the creatures individual mutational prowess
    float mutability;
    private int AmountOfNodes;
    private int AmountOfMuscles;

    /// <summary>
    /// Returns the amount of nodes present in the nodelist
    /// </summary>
    int nodeCount
    {
        get { return nodes.Count; }
    }

    /// <summary>
    /// Returns the amount of muscles present in the musclelist
    /// </summary>
    int muscleCount
    {
        get { return muscles.Count; }
    }

    /// <summary>
    /// Returns the chance of a mutation occuring, a factor of the mutation constant and the mutability property
    /// </summary>
    float mutationChance
    {
        get { return mutationChance; }
    }

    /// <summary>
    /// Returns the average position of all the creatures nodes combined
    /// </summary>
    Vector2 pos // The position of the creature is determined by the average position of all it's nodes
    {
        get
        {
            // get and save the number of nodes this creature has
            float nodeCount = this.nodeCount;

            // if there's 0 or less nodes, abort and return an empty vector
            if (nodeCount <= 0)
                return Vector2.zero;

            // initialize and normalize a new vector savepoint
            Vector2 combinedPos = Vector2.zero;

            // for each node the creature has
            for (int i = 0; i < nodeCount; i++)
            {
                // add all the node positions together into the vector savepoints
                Vector2 thisNodePos = nodes[i].Pos;
                combinedPos += thisNodePos;
            }

            // to get the average value, subtract the total by the contributor count
            return (combinedPos /= nodeCount);
        }
    }

    float MaxX
    {
        get
        {
            return nodes.OrderByDescending(n => n.transform.position.x).FirstOrDefault().transform.position.x;
        }
    }
    float MaxY
    {
        get
        {
            return nodes.OrderByDescending(n => n.transform.position.y).FirstOrDefault().transform.position.y;
        }
    }
    float MinX
    {
        get
        {
            return nodes.OrderByDescending(n => n.transform.position.x).LastOrDefault().transform.position.x;
        }
    }
    float MinY
    {
        get
        {
            return nodes.OrderByDescending(n => n.transform.position.y).LastOrDefault().transform.position.y;
        }
    }


    /// <summary>
    /// Used to spontaneously randomize
    /// </summary>
    /// <returns>float value between -1 and 1, to the power of 19</returns>
    public static float randGet()
    {
        return Mathf.Pow(Random.Range(-1, 1), 19);
    }

    private float sigmoid(float f)
    {
        return 1 / (1 + Mathf.Pow((float)System.Math.E, -f));
    }

    int getNewMuscleAxon(int nodeNumber)
    {
        // if the pseudo-random float value is smaller than 0.5 (~50% chance)
        if (Random.Range(0f, 1f) < 0.5)
            // return a random int value between 0 and the number recieved
            return Random.Range(0, nodeNumber);

        else // else return -1
            return -1;
    }
    public IEnumerator Wait(float delayInSecs)
    {
        yield return new WaitForSeconds(delayInSecs);
    }
    /// <summary>
    /// Creature constructor
    /// </summary>
    public Creature() { }
    public void SetInititationValues(int ID, List<Node> nodes, List<Muscle> muscles, float cFitness, bool alive, float creatureTimer, float mutability)
    {
        this.ID = ID;
        this.nodes = nodes;
        this.muscles = muscles;
        this.cFitness = cFitness;
        this.alive = alive;
        this.creatureTimer = creatureTimer;
        this.mutability = mutability;
    }
    public void SetInititationValues(int ID, float cFitness, bool alive, float creatureTimer, float mutability)
    {
        this.ID = ID;
        this.cFitness = cFitness;
        this.alive = alive;
        this.creatureTimer = creatureTimer;
        this.mutability = mutability;
    }


    #region NEAT implementation
    void Start()
    {
        inputArr = box.InputSignalArray;
        inputArr[3] = timer;
        box.Activate();
        outputArr = box.OutputSignalArray;
        gameObject.name = CreatureHolder.Name(UnityEngine.Random.Range(0, 5162));
        AmountOfNodes = Mathf.Clamp(Mathf.RoundToInt(((float)outputArr[TestSettings.Instance.MaxMuscles]) * TestSettings.Instance.MaxNodes), 3, TestSettings.Instance.MaxNodes);
        AmountOfMuscles = Mathf.Clamp(Mathf.RoundToInt((float)(outputArr[TestSettings.Instance.MaxMuscles + 1]) * TestSettings.Instance.MaxMuscles), 2, TestSettings.Instance.MaxMuscles);
        nodes = new List<Node>();
        muscles = new List<Muscle>();
        CreateNodes();
        CreateMuscles();
        checkForOverlap();
        checkForLoneNodes();
        transform.position = new Vector3(0, pos.y - MinY + 0.8f, transform.position.z); //places the creature 0.5 units above ground
        StartCoroutine(RunSim());
        for (int i = 0; i < muscles.Count; i++)
        {
            StartCoroutine(muscles[i].exertForce(nodes));
        }
    }
    private IEnumerator RunSim()
    {
        while(IsRunning)
        {
            outputArr = box.OutputSignalArray;
            inputArr = box.InputSignalArray;
            inputArr[0] = pos.x;
            inputArr[1] = pos.y;
            inputArr[2] = nodes.Count;
            inputArr[3] = timer;
            inputArr[4] = MaxX;
            inputArr[5] = MaxY;
            inputArr[6] = MinX;
            inputArr[7] = MinY;
            box.Activate();
            for (int i = 0; i < muscles.Count; i++)
            {
                muscles[i].Length = (float)outputArr[i] * TestSettings.Instance.lMax;
                //print((float)outputArr[i] * TestSettings.Instance.lMax);
                //muscles[i].exertForce(nodes);
            }

            foreach (Node node in nodes)
            {
                //node.ApplyForces();

            }
            timer += 2;
            yield return new WaitForSeconds(0.2f);
        }
    }
    //void FixedUpdate()
    //{
    //    if (IsRunning)
    //    {
    //        outputArr = box.OutputSignalArray;
    //        ISignalArray inputArr = box.InputSignalArray;
    //        inputArr[0] = pos.x;
    //        inputArr[1] = pos.y;
    //        inputArr[2] = nodes.Count;
    //        inputArr[3] = timer;
    //        inputArr[4] = MaxX;
    //        inputArr[5] = MaxY;
    //        inputArr[6] = MinX;
    //        inputArr[7] = MinY;
    //        box.Activate();

    //        for (int i = 0; i < AmountOfMuscles; i++)
    //        {
    //            //muscles[i].Length = (float)outputArr[i] * TestSettings.Instance.lMax;
    //            //muscles[i].exertForce(nodes);
    //        }

    //        foreach (Node node in nodes)
    //        {
    //            node.ApplyForces();

    //        }

    //        timer += 0.1f;
    //    }
    //}

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        IsRunning = true;
    }

    public override float GetFitness()
    {
        fitness = MaxX;
        if (fitness < 0)
            return 0;
        return (float)System.Math.Round(fitness, 5);
    }
    #endregion


    public GameObject copy(int newID)
    {
        GameObject newCreature = new GameObject();

        Creature creature = newCreature.AddComponent<Creature>();
        List<Node> newNodes = new List<Node>(0);
        List<Muscle> newMuscles = new List<Muscle>(0);

        for (int i = 0; i < nodeCount; i++)
        {
            Node node = nodes[i];
            CreateNode(node.Pos, newCreature.transform, node.prevX,node.prevY, node.M, node.friction, node.value, node.operation, node.axon1, node.axon2);
        }

        for (int i = 0; i < muscleCount; i++)
        {
            Muscle cM = muscles[i].copy();
            CreateMuscle(newCreature.transform, cM.axon, cM.p1, cM.p2, cM.Length, cM.rigidity);
        }

        if (newID == -1)
            newID = ID;
        creature.SetInititationValues(newID, newNodes, newMuscles, cFitness, alive, creatureTimer, mutability);

        return newCreature;
    }


    public Creature modify(int ID)
    {
        // create a new instance of creature to be modified
        Creature modifiedCreature = CreatureHolder.CreateCreature(Vector3.zero, ID, new List<Node>(0), new List<Muscle>(0), 0, true, creatureTimer + randGet() * 16 * mutability, Mathf.Min(mutability * Random.Range(0.8f, 1.25f), 2));

        // add all the nodes of the current creature to the new creature
        for (int i = 0; i < nodeCount; i++)
        {
            Node temp = nodes[i];
            Node node = modifiedCreature.CreateNode(Vector2.zero, gameObject.transform, temp.prevX, temp.prevY, temp.M, temp.friction, temp.value, temp.operation, temp.axon1, temp.axon2);
            node.modify(mutability, nodeCount, mutationChance, operationCount);
        }

        // add all the muscles of the current creature to the new creatures
        for (int i = 0; i < muscles.Count; i++)
        {
            Muscle temp = muscles[i];
            Muscle muscle = modifiedCreature.CreateMuscle(gameObject.transform, temp.axon, temp.p1, temp.p2, temp.Length, temp.rigidity);
            muscle.modify(nodeCount, mutability, mutationChance, nodes);
        }

        // if a random float between 0 and 1 is larger then the mutationChance of the creature, or if the creature has 2 or less nodes
        if (Random.Range(0f, 1f) < mutationChance || nodeCount <= 2)
            modifiedCreature.addRandNode(gameObject.transform, 0, 0, 0.4f, Random.Range(0f, 1f), Random.Range(0f, 1f), (int)Mathf.Floor(Random.Range(0f, operationCount)),
                  Random.Range(0, nodeCount), Random.Range(0, nodeCount)); // add a new node

        // if a random float between 0 and 1 is larger then the mutationChance of the creature
        if (Random.Range(0f, 1f) < mutationChance)
            modifiedCreature.addRandMuscle(gameObject.transform, -1, -1); // add a new muscle connecting two of the nodes

        // if a random float between 0 and 1 is larger then the mutationChance of the creature
        if (Random.Range(0f, 1f) < mutationChance && modifiedCreature.nodeCount >= 4)
            modifiedCreature.removeRandNode(); // remove a random node


        // if a random float between 0 and 1 is larger then the mutationChance of the creature
        if (Random.Range(0f, 1f) < mutationChance && modifiedCreature.muscleCount >= 2)
            modifiedCreature.removeRandMuscle(); // remove a random muscle

        modifiedCreature.checkForOverlap();
        modifiedCreature.checkForLoneNodes();
        modifiedCreature.checkForBadAxons();

        return modifiedCreature;
    }

    void checkForOverlap()
    {
        List<int> overlappingNodes = new List<int>();

        for (int i = 0; i < muscleCount; i++)
        {
            for (int j = i + 1; j < muscleCount; j++)
            {
                if (muscles[i].p1 == muscles[j].p1 && muscles[i].p2 == muscles[j].p2)
                    overlappingNodes.Add(i);

                else if (muscles[i].p1 == muscles[j].p2 && muscles[i].p2 == muscles[i].p1) 
                    overlappingNodes.Add(i);

                else if (muscles[i].p1 == muscles[i].p2) //Binded to itself
                    overlappingNodes.Add(i);
            }
        }

        for (int i = overlappingNodes.Count - 1; i >= 0; i--)
        {
            int b = overlappingNodes[i];
            if (b < muscleCount)
            {
                GameObject obj = muscles[b].gameObject;
                muscles.Remove(muscles[b]);
                Destroy(obj);
            }
        }

    }

    void checkForLoneNodes()
    {
        if (nodeCount >= 3)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                int connections = 0;
                int connectedTo = -1;

                for (int j = 0; j < muscleCount; j++)
                {
                    if (muscles[j].p1.ID == i || muscles[j].p2.ID == i)
                    {
                        connections++;
                        connectedTo = j;
                    }
                }
                
                if (connections <= 1)
                {
                    int newConnectionNode = (int)Mathf.Floor(Random.Range(0f, nodeCount));

                    while (newConnectionNode == i || newConnectionNode == connectedTo)
                        newConnectionNode = (int)Mathf.Floor(Random.Range(0f, nodeCount));

                    addRandMuscle(gameObject.transform, i, newConnectionNode);
                }
            }
        }
    }

    void checkForBadAxons()
    {
        for (int i = 0; i < nodeCount; i++)
        {
            Node node = nodes[i];

            if (node.axon1 >= nodeCount)
                node.axon1 = Random.Range(0, nodeCount);

            if (node.axon2 >= nodeCount)
                node.axon2 = Random.Range(0, nodeCount);

            node.safeInput = (TestSettings.Instance.OperationAxons[node.operation] == 0);
        }

        for (int i = 0; i < muscleCount; i++)
        {
            Muscle muscle = muscles[i];

            if (muscle.axon >= nodeCount)
                muscle.axon = getNewMuscleAxon(nodeCount);
        }
        bool changed = false;

        while (true)
        {
            changed = false;

            for (int i = 0; i < nodeCount; i++)
            {
                Node node = nodes[i];

                if (!node.safeInput)
                {
                    if ((TestSettings.Instance.OperationAxons[node.operation] == 1 && nodes[node.axon1].safeInput) ||
                        (TestSettings.Instance.OperationAxons[node.operation] == 2 && nodes[node.axon1].safeInput &&
                         nodes[node.axon2].safeInput))
                    {
                        node.safeInput = true;
                        changed = true;
                    }
                }
            }
            if (!changed)
                break;
        }
        for (int i = 0; i < nodeCount; i++)
        {
            Node node = nodes[i];

            if (!node.safeInput)
            {
                node.operation = 0;
                node.value = Random.Range(0f, 1f);
            }
        }
    }

    #region Add and Remove Nodes
    private void CreateNodes()
    {
        CreateNode(transform.position, gameObject.transform, UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), //Create our "main" node on the location of the creature
             0.4f, UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1),
                 Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)),
                      Mathf.FloorToInt(UnityEngine.Random.Range(0, AmountOfNodes)),
                          Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)));

        for (int i = 1; i <= AmountOfNodes; i++) //Create the additional nodes 
        {
             addRandNode(gameObject.transform, UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1),
                0.4f, UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1),
                    Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)),
                        Mathf.FloorToInt(UnityEngine.Random.Range(0, AmountOfNodes)),
                           Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)));
        }
    }

    /// <summary>
    /// Creates a new pseudo-randomized node for the creature
    /// </summary>
    public void addRandNode(Transform parent, float prevX, float prevY, float M, float friction, float value, int operation, int axon1, int axon2)
    {
        //int parentNode = Random.Range(0, nodeCount - 1);
        GameObject parentNode = nodes.OrderByDescending(n => n.transform.position.x).FirstOrDefault().gameObject;
        // since floor takes the smallest whole number, and the random range can only come infinitely close to nodeCount, this is not an issue*
        // might still be a problem because unity random float might include the max in the equation

        // an angle between 0 - 180 degrees, in radians
        float angle = Mathf.Clamp(Mathf.PI * (nodes.Count / AmountOfNodes),0,Mathf.PI);

        // a float between minrad and maxRad
        //float distance = Random.Range(TestSettings.Instance.NodeMinRad, TestSettings.Instance.NodeMaxRad);
        float distance = TestSettings.Instance.NodeMinRad;

        // sets the x and y cordinates of the new node to be close to the randomly selected parent node
        float x = parentNode.transform.position.x + Mathf.Cos(angle) * distance;
        float y = parentNode.transform.position.y + Mathf.Sin(angle) * distance;

        // Also places the newly created node into the creatures list of nodes
        Node node = CreateNode(new Vector2(x,y), parent, prevX, prevY, M, friction, value, operation, axon1, axon2);

        //int nextClosestNode = 0;

        //float record = float.MaxValue; //For the first run to always be true

        // search the node list for the closest node, -1 excludes the recently made one
        //for (int i = 0; i < nodeCount-1; i++)
        //{
        //    // the parent node is already close, we want the closest node that isn't the parent node
        //    if (i != parentNode)
        //    {
        //        // compare the distance from the newly created node to the currently inspected node
        //        float distX = nodes[i].transform.position.x - x;
        //        float distY = nodes[i].transform.position.y - y;

        //        float hypotenuse = Mathf.Sqrt((distX * distX) + (distY * distY));
        //        if (hypotenuse < record)
        //        {
        //            record = hypotenuse;
        //            nextClosestNode = i;
        //        }
        //    }
        //}
        //// try to add a muscle between the parent and the new node
        //addRandMuscle(gameObject.transform, node.ID, parentNode);
        //// try to add a muscle between the other close node and the new node
        //addRandMuscle(gameObject.transform, node.ID, nextClosestNode); 
    }


    /// <summary>
    /// Creates a new node at target location
    /// </summary>
    public Node CreateNode(Vector2 pos, Transform parent, float prevX, float prevY, float M, float friction, float value, int operation, int axon1, int axon2)
    {
        Node node;
        GameObject obj = Instantiate(TestSettings.Instance.NodePrefab, new Vector3(pos.x,Mathf.Clamp(pos.y,0.8f, 10f),TestSettings.Instance.ZValue), Quaternion.identity) as GameObject;
        obj.transform.SetParent(parent); //Sets the node's parent
        if (!obj.GetComponent<Node>())
        {
            node = obj.AddComponent<Node>();
            node.SetInitiationValues(prevX, prevY, M, friction, value, operation, axon1, axon2);
        }
        else
        {
            node = obj.GetComponent<Node>();
            node.SetInitiationValues(prevX, prevY, M, friction, value, operation, axon1, axon2);
        }
        nodes.Add(node);
        node.gameObject.name = "Node " + nodeCount;
        node.ID = nodeCount - 1;
        return node;
    }
    /// <summary>
    /// Creates a new Muscle
    /// </summary>
    public Muscle CreateMuscle(Transform parent, int axon, Node node1, Node node2, float lenght, float rigidity)
    {
        Muscle muscle;
        GameObject obj = Instantiate(TestSettings.Instance.MusclePrefab, new Vector3(pos.x, pos.y, TestSettings.Instance.ZValue), Quaternion.identity) as GameObject;
        obj.transform.SetParent(parent);
        if (!obj.GetComponent<Muscle>())
        {
            muscle = obj.AddComponent<Muscle>();
            muscle.SetInitiationValues(axon, node1, node2, lenght, rigidity);
        }
        else
        {
            muscle = obj.GetComponent<Muscle>();
            muscle.SetInitiationValues(axon, node1, node2, lenght, rigidity);
        }
        muscles.Add(muscle);
        muscle.gameObject.name = "Muscle " + muscles.Count;
        return muscle;
    }


    /// <summary>
    /// Removes a pseudo randomized node from the creature
    /// </summary>
    void removeRandNode()
    {
        int removed = (int)Mathf.Floor(Random.Range(0f, nodeCount));

        Node removedNode = nodes[removed];

        for (int i = 0; i < muscleCount;)
        {
            if (muscles[i].p1 == removedNode || muscles[i].p2 == removedNode)
                muscles.Remove(muscles[i]);
            else
                i++;
        }
       
        // fills in the gap between the ID's by lowering the id that are larger than the removed node
        for (int i = 0; i < muscleCount; i++)
        {
            if (muscles[i].p1.ID >= removed)
                muscles[i].p1.ID--;

            if (muscles[i].p2.ID >= removed)
                muscles[i].p2.ID--;
        }
        GameObject obj = nodes[removed].gameObject;
        nodes.Remove(nodes[removed]);
        Destroy(obj);
    }
    #endregion

    #region Add and Remove Muscles
    public void CreateMuscles()
    {
        //for (int i = 0; i <= AmountOfMuscles; i++)
        //{
        //    int node;
        //    int node2;

        //    if (i < AmountOfNodes - 1)
        //    {
        //        node = i;
        //        node2 = i + 1;
        //    }
        //    else
        //    {
        //        node = UnityEngine.Random.Range(0, AmountOfNodes);
        //        node2 = node;
        //        while (node == node2)
        //        {
        //            node2 = UnityEngine.Random.Range(0, AmountOfNodes);
        //        }
        //    }
        //    float s = 0.8f;
        //    if (i >= 10)
        //    {
        //        s *= 1.414f;
        //    }
        //    float length = UnityEngine.Random.Range(0.5f, 1.5f);
        //    addRandMuscle(gameObject.transform, node, node2);
        //}
        for (int i = 0; i <= AmountOfMuscles; i++)
        {
            int node;
            int node2;

            if (i < nodes.Count - 1)
            {
                node = i;
                node2 = i + 1;
            }
            else
            {
                node = Mathf.Clamp(i - nodes.Count, 0, nodes.Count - 1);
                node2 = (nodes.Count - 1) - node;
            }
            addRandMuscle(gameObject.transform, node, node2);
        }
    }
    public void addRandMuscle(Transform parent, int node1, int node2)
    {
        if (muscles.Count > TestSettings.Instance.MaxMuscles)
        {
            return;
        }

        int axon = getNewMuscleAxon(nodeCount);

        if (node1 < 0 || node1 > nodes.Count - 1 || node2 < 0 || node2 > nodes.Count - 1)
        {
            return;
        }

        if (node1 != -1)
        {
            CreateMuscle(gameObject.transform, axon, nodes[node1], nodes[node2], 0, Random.Range(0.02f, 0.08f));
        }
    }

    void removeRandMuscle()
    {
        int removed = (int)Mathf.Floor(Random.Range(0f, muscleCount));
        Muscle muscle = muscles[removed]; 
        muscles.Remove(muscle);
        Destroy(muscle);
    }
    #endregion
}
