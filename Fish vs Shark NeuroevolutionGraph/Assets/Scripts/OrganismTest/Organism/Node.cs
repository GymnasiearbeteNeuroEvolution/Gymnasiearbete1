using UnityEngine;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Node, the "feet" of an organism;
/// Nodes are the subjects of physics forces such as fricion and gravity;
/// Nodes collide with the ground, but not with eachother;
/// Two nodes can be connected with a muscle; 
/// </summary>
class Node : MonoBehaviour
{
    public int ID;

    private Rigidbody body;

    Vector2 pos { get { return new Vector2(gameObject.transform.position.x, gameObject.transform.position.y); } } // the position of this node
    public Vector2 Pos { get { return pos; } } // the publically accessible shortcut to the nodes position
    public float X { get { return gameObject.transform.position.x; } } // shortcut that returns the X value of the node position
    public float Y { get { return pos.y; } } // shortcut that returns the Y value of the node position

    Vector2 prevPos; // the previous position of this node
    public Vector2 PrevPos { get { return prevPos; } } // the publically accessible shortcut to the nodes previous position
    public float prevX { get { return prevPos.x; } set { prevPos.x = value; } } // shortcut to prev X
    public float prevY { get { return prevPos.y; } set { prevPos.y = value; } } // shortcut to prev Y

    /// <summary>
    /// The nodes current velocity in the respective axis
    /// </summary>
    Vector2 velocity; // the velocity of this node
    public Vector2 Velocity { get { return velocity; } } // the publically accessible shortcut to the nodes velocity
    public float velX { get { return velocity.x; } set { velocity.x = value; } } // shortcut to velX
    public float velY { get { return velocity.y; } set { velocity.y = value; } } // shortcut to velY

    /// <summary>
    /// The nodes previous velocity in the respective axis
    /// </summary>
    Vector2 prevVelocity; // the previous velocity of this node
    public Vector2 PrevVelocity { get { return prevVelocity; } } // the publically accessible shortcut to the nodes previous velocity
    public float prevVelX { get { return prevVelocity.x; } set { prevVelocity.x = value; } } // shortcut to prev velX
    public float prevVelY { get { return prevVelocity.y; } set { prevVelocity.y = value; } } // shortcut to prev velY

    // 
    public float M;

    /// <summary>
    /// friction koefficient, value between 0 <-> 1, 0 being no friction and 1 being complete friction
    /// </summary>
    public float friction; // friction is shown by the color of the node, from white (no friction) to black (complete friction)

    //
    public float value;

    //
    public bool safeInput;

    //
    public int operation;

    //The ID of the first axon, if any
    public int axon1;

    //The ID of the second axon, if any 
    public int axon2;

    /// <summary>
    /// Used to spontaneously randomize
    /// </summary>
    /// <returns>float value between -1 and 1, to the power of 19</returns>
    float randGet()
    {
        return Mathf.Pow(Random.Range(-1, 1), 19);
    }

    protected Node() { }
    
    void Awake()
    {
        body = GetComponent<Rigidbody>();

        IEnumerable<Collider> ignore = from a in GameObject.FindGameObjectsWithTag("Node") where a.transform.parent != transform.parent select a.GetComponent<Collider>();
        foreach(Collider coll in ignore)
        {
            Physics.IgnoreCollision(coll, GetComponent<Collider>(), true);
        }
    }

    public void SetInitiationValues(float prevX, float prevY, float M, float friction, float value, int operation, int axon1, int axon2)
    {
        this.prevX = prevX;
        this.prevY = prevY;
        this.M = M;
        this.friction = friction;
        this.value = value;
        this.operation = operation;
        this.axon1 = axon1;
        this.axon2 = axon2;

        body = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Make modifications to the current node
    /// </summary>
    /// <param name="mutability">mutation factor</param>
    /// <param name="nodeNumber">the node number</param>
    /// <returns>Returns a modified Node</returns>
    public Node modify(float mutability, int nodeNumber, float mutationChance, int operationCount)
    {
        // saves modifications to the x, y and m values
        float newX = X + randGet() * 0.5f * mutability;
        float newY = Y + randGet() * 0.5f * mutability;
        float newM = M + randGet() * 0.1f * mutability;

        //newM = 0.4f;
        newM = Mathf.Clamp(newM, 0.3f, 0.5f);
        // ---- //

        float newValue = value * (1 + randGet() * 0.2f * mutability);
        int newOperation = operation;
        int newAxon1 = axon1;
        int newAxon2 = axon2;

        // if a random integrer between 0 and 1 manages to be smaller than 0.06 * mutability, __
        if (Random.Range(0, 1) < mutationChance)
            // sets a value between 0 and 12
            newOperation = Random.Range(0, operationCount);

        if (Random.Range(0, 1) < mutationChance)
            // sets a value between 0 and the passed value nodeNumber
            newAxon1 = Random.Range(0, nodeNumber);

        if (Random.Range(0, 1) < mutationChance)
            // sets a value between 0 and the passed value nodeNumber
            newAxon2 = Random.Range(0, nodeNumber);

        // if the value between 0 and 12 is 1 (1 in 12 chance || 8.33%)
        if (newOperation == 1)
            newValue = 0; // set newValue to 0

        // else if the value is 2 (1 in 12 chance || 8.33%)
        else if (newOperation == 2)
            newValue = newX * 0.2f; // set newValue to 1/5 of newX

        // else if the value is 3 (1 in 12 chance || 8.33%)
        else if (newOperation == 3)
            newValue = -newY * 0.2f; //set newValue to -1/5 of newY
                                     // chance of newValue staying the same = 3 in 4 chance or 75%

        Node newNode = new Node();
        newNode.SetInitiationValues(0, 0, M, friction, value, operation, axon1, axon2);
        return newNode;
    }
    public void ApplyForces()
    {
        if(body.velocity.magnitude > TestSettings.Instance.MaxNodeSpeed)
        {
            body.velocity = body.velocity.normalized * TestSettings.Instance.MaxNodeSpeed;
        }
    }
}
