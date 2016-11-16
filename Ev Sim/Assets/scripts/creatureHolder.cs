using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class creatureHolder : MonoBehaviour {

    // Use this for initialization
    List<Creature> creatureList;

    const float bigMutationChance = 0.06f;
    const int operationCount = 12;

    void Start ()
    {
        creatureList = new List<Creature>();
	}

    class Creature
    {
        
    }

    /// <summary>
    /// Node, the "feet" of an organism
    /// </summary>
    class Node
    {
        /// <summary>
        /// Return a float value between -1 and 1, to the power of 19
        /// </summary>
        /// <returns></returns>
        float rand()
        {
            return Mathf.Pow(Random.Range(-1, 1), 19);
        }

        // declare all the node variables
        float x, y, vx, vy, prevX, prevY, prevVX, prevVy, m, f, value, valueToBe;

        int operation, axon1, axon2;

        float pressure;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tx">target X</param>
        /// <param name="ty">target Y</param>
        /// <param name="tvx">target VX</param>
        /// <param name="tvy">target VY</param>
        /// <param name="tm">target M</param>
        /// <param name="tf">target F</param>
        /// <param name="val">target Value</param>
        /// <param name="operation"></param>
        /// <param name="axon1"></param>
        /// <param name="axon2"></param>
        Node (float tx, float ty, float tvx, float tvy, float tm, float tf, float val, int operation, int axon1, int axon2)
        {
            prevX = x = tx;
            prevY = y = ty;
            prevVX = vx = tvx;
            prevVy = vy = tvy;
            m = tm;
            f = tf;
            value = valueToBe = val;
            this.operation = operation;
            this.axon1 = axon1;
            this.axon2 = axon2;
            pressure = 0;
        }

        /// <summary>
        /// Makes a copy of the current node and returns it
        /// </summary>
        /// <returns></returns>
        Node copy()
        {
            return (new Node());
        }

        /// <summary>
        /// Make modifications to the current node
        /// </summary>
        /// <param name="mutability">mutation factor</param>
        /// <param name="nodeNumber">the node number</param>
        /// <returns></returns>
        Node modify(float mutability, int nodeNumber)
        {
            // saves modifications to the x, y and m values
            float newX = x + rand() * 0.5f * mutability;
            float newY = y + rand() * 0.5f * mutability;
            float newM = m + rand() * 0.1f * mutability;

            // questionable application
            newM = Mathf.Min(Mathf.Max(newM, 0.3f), 0.5f);
            newM = 0.4f;
            // ---- //

            // __
            float newValue = value * (1 + rand() * 0.2f * mutability);
            int newOperation = operation;
            int newAxon1 = axon1;
            int newAxon2 = axon2;

            // if a random integrer between 0 and 1 manages to be smaller than 0.06 * mutability, __
            if (Random.Range(0, 1) < bigMutationChance * mutability)
                // sets a value between 0 and 12
                newOperation = Random.Range(0, operationCount);

            if (Random.Range(0, 1) < bigMutationChance * mutability)
                // sets a value between 0 and the passed value nodeNumber
                newAxon1 = Random.Range(0, nodeNumber);

            if (Random.Range(0, 1) < bigMutationChance * mutability)
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

            Node newNode = new Node()
        }
    }

    /// <summary>
    /// Muscle connecting the nodes, capable of contracting and expanding
    /// </summary>
    class Muscle
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
