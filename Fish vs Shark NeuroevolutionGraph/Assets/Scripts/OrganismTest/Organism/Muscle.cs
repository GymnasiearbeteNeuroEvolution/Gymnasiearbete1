using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Muscle connecting the nodes;
/// muscles are capable of contracting and expanding;
/// they have an extended lenght (TestSettings.Instance.lMax);
/// and a contracted length (TestSettings.Instance.lMin);
/// they also have an extended time (tMax);
/// and a contracted time (tMin), both based on the internal clock;
/// finally, muscles have strenght, how hard it can push or pull to reach TestSettings.Instance.lMin or TestSettings.Instance.lMax
/// </summary>
class Muscle : MonoBehaviour
{
    // axon refers to a node, probably the 
    public int axon;

    // the two nodes the muscle joins together, whom will feel the exerted force
    public Node p1, p2;

    private float length;
    /// <summary>
    /// The current length of the muscle
    /// </summary>
    public float Length
    {
        get { return length; }
        set { length = Mathf.Clamp(value, TestSettings.Instance.lMin, TestSettings.Instance.lMax); }
    }
    public float displayLength;

    public float HalfLength
    {
        get { return (TestSettings.Instance.lMax - TestSettings.Instance.lMin) / 2; }
    }

    /// <summary>
    /// Returns the percentage of the current rigidity relative to the maximum 
    /// </summary>
    private float peakRigid(float f)
    {
        float max = getRigidityValue(TestSettings.Instance.lMax);
        return f / max;
    }

    
    float prevTargetLength;



    // the relative intervals of contraction abd extension respectively 
    // both make up two sides of a whole, and can be described as the ratio of one hearthbeat (timestep)
    // thus they should together be a total of 1 
    float tMax, tMin;

    // strenght is shown by the muscles opaque-ness, the more opaque the stronger
    // the stronger the muscle is the closer it can reach TestSettings.Instance.lMin and TestSettings.Instance.lMax and the faster it gets there
    float strenght;

    // stiffness, the property of the muscle to resist deformation
    public float rigidity;
    internal double TargetLength;

    protected Muscle() { }

    public void SetInitiationValues(int axon, Node node1, Node node2, float lenght, float rigidity)
    {
        this.axon = axon;

        p1 = node1;
        p2 = node2;

        prevTargetLength = this.Length = length;

        this.rigidity = rigidity;
    }
    void Start()
    {
        StartCoroutine(updatePos());
    }

    void Update()
    {

        displayLength = Length;
    }

    private void limitNode()
    {
        //if(transform.localScale.y >= TestSettings.Instance.lMax || transform.localScale.y <= TestSettings.Instance.lMin)
        //{
        //    float force =1;
        //    float angle = Mathf.Atan2(p1.Y - p2.Y, p1.X - p2.X);
        //    p1.GetComponent<Rigidbody>().AddForce(-new Vector3(force * Mathf.Cos(-angle), force * Mathf.Sin(angle), 0) * 1000);
        //    p2.GetComponent<Rigidbody>().AddForce(new Vector3(force * Mathf.Cos(angle), force * Mathf.Sin(angle), 0) * 1000);

        //}
    }

    public float GetRigidity(float l)
    {
        float rigid = getRigidityValue(l);
        rigid = peakRigid(rigid);
        System.Console.WriteLine(rigid);
        return rigid;
    }

    /// <summary>
    /// Rigidity is defined as an exponential function 
    /// </summary>
    private float getRigidityValue(float f)
    {
        float value = f - (TestSettings.Instance.lMax - HalfLength);
        value *= value;
        return value;
    }

    /// <summary>
    /// Makes a copy of the current muscle
    /// </summary>
    /// <returns>Muscle</returns>
    public Muscle copy()
    {
        Muscle muscle = new Muscle();
        muscle.SetInitiationValues(axon, p1, p2, length, rigidity);
        return muscle;
    }

    /// <summary>
    /// Makes a mutation of the current node
    /// </summary>
    /// <returns>Muscle</returns>
    public Muscle modify(int nodeNum, float mutability, float mutationChance, List<Node> nodes)
    {
        // makes a new instance of the parent1, parent2 and axon values
        Node newp1 = p1;
        Node newp2 = p2;
        int newAxon = axon;

        if (Random.Range(0f, 1f) < mutationChance)
            newp1 = nodes[Random.Range(0, nodeNum)];

        if (Random.Range(0f, 1f) < mutationChance)
            newp2 = nodes[Random.Range(0, nodeNum)];

        if (Random.Range(0f, 1f) < mutationChance)
            newAxon = getNewMuscleAxon(nodeNum);

        float newRigidity = Mathf.Min(Mathf.Max(rigidity * (1 + Creature.randGet() * 0.9f * mutability), 0.01f), 0.08f);
        float newLenght = Mathf.Min(Mathf.Max(length + Creature.randGet() * mutability, 0.4f), 1.25f);

        Muscle newMuscle = new Muscle();
        newMuscle.SetInitiationValues(newAxon, newp1, newp2, newLenght, newRigidity);
        return newMuscle;
    }

    public static int getNewMuscleAxon(int nodeNum)
    {
        // gives a 50% chance of the muscle binding to a new axon
        if (Random.Range(0f, 1f) < 0.5f)
            return Random.Range(0, nodeNum);

        else
            return -1;
    }
    /// <summary>
    /// Clamps a float between 0.5 and 1.5. 
    /// </summary>
    float clampLength(float f)
    {
        return Mathf.Clamp(f, 0.5f, 1.5f);
    }

    private IEnumerator updatePos()
    {
        while (gameObject.activeInHierarchy)
        {
            Vector2 diff = p2.transform.position - p1.transform.position;
            float hyp = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.y, 2));
            float rot = 0;
            transform.localScale = new Vector3(transform.localScale.x, hyp / 2, transform.localScale.z);
            if(diff.x != 0)
            rot = Mathf.Atan(diff.y / diff.x);
            transform.position = (p1.transform.position + p2.transform.position) / 2;
            int asd;
            if (rot > (3 * Mathf.PI / 2))
                asd = -90;
            else
                asd = 90;
            transform.localRotation = Quaternion.Euler(0, 0, asd + (rot * 180) / Mathf.PI);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Contracts or extends the current muscle
    /// </summary>
    public IEnumerator exertForce(List<Node> nodes)
    {
        while(gameObject.activeInHierarchy)
        {
            float targetLength = prevTargetLength;

            //if (axon >= 0 && axon < nodes.Count)
            //    targetLength = length * clampLength(nodes[axon].value);

            //else
            targetLength = length;

            // the distance between the two connected parent nodes
            float distance = Vector2.Distance(p1.Pos, p2.Pos);

            // the angle from one node to the other
            float angle = Mathf.Atan2(p1.Y - p2.Y, p1.X - p2.X);

            //float angle = Mathf.Deg2Rad * Vector2.Angle(new Vector2(p1.Pos.x, p1.Pos.y),new Vector2(p2.Pos.x, p2.Pos.y));

            // the force that will be exerted
            float force = Mathf.Min(Mathf.Max(1 - (distance / targetLength), -5f), 5f);

            p1.GetComponent<Rigidbody>().AddForce(new Vector3(force * Mathf.Cos(angle), force * Mathf.Sin(angle), 0));
            p2.GetComponent<Rigidbody>().AddForce(-new Vector3(force * Mathf.Cos(angle), force * Mathf.Sin(angle), 0));

            //p1.GetComponent<Rigidbody>().velocity += (new Vector3(force * Mathf.Cos(angle), force * Mathf.Sin(angle), 0) ) * 10;
            //p2.GetComponent<Rigidbody>().velocity -= new Vector3(force * Mathf.Cos(angle), force * Mathf.Sin(angle), 0) * 10;


            //// the resulting velocity in both x and y axis for parent node 1
            //p1.velX += Mathf.Cos(angle) * force * rigidity / p1.M;
            //p1.velY += Mathf.Sin(angle) * force * rigidity / p1.M;

            //// the resulting velocity in both x and y axis for parent node 2
            //p2.velX -= Mathf.Cos(angle) * force * rigidity / p2.M;
            //p2.velY -= Mathf.Sin(angle) * force * rigidity / p2.M;

            prevTargetLength = targetLength;
            yield return new WaitForSeconds(0.05f);
        }

    }
}