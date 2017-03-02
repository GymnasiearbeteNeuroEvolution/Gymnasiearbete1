using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

class CreatureHolder : MonoBehaviour
{
    // Use this for initialization
    List<Creature> creatureList;
    static string[] Names;

    void Awake()
    {
        try
        {
            Names = File.ReadAllLines(Application.dataPath + "/Resources/Names.txt");
        }
        catch(Exception e)
        {
            print(e);
        }
    }
    void Start()
    {

    }
    
    //public static void LaunchCreatureTest()
    //{
    //    for (int y = 0; y < 1; y++) //
    //    {
    //        Creature creature = CreateCreature(new Vector2(0, 6));
    //        creature.gameObject.name = Name(UnityEngine.Random.Range(0, 5162));
    //        int nodeNum = UnityEngine.Random.Range(2, TestSettings.Instance.MaxNodes); //Creates between 3 and MaxNodes nodes 
    //        int muscleNum = UnityEngine.Random.Range(nodeNum - 1, TestSettings.Instance.MaxMuscles); //Creates between 2 and MaxMuscles muscles 
    //        creature.CreateNode(creature.transform.position, creature.gameObject.transform, UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), //Create our "main" node on the location of the creature
    //             0.4f, UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1),
    //                 Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)),
    //                      Mathf.FloorToInt(UnityEngine.Random.Range(0, nodeNum)),
    //                          Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)));

    //        for (int i = 1; i <= TestSettings.Instance.MaxNodes; i++) //Create the additional nodes 
    //        {
    //            creature.addRandNode(creature.gameObject.transform, UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1),
    //                0.4f, UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1),
    //                    Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)),
    //                        Mathf.FloorToInt(UnityEngine.Random.Range(0, nodeNum)),
    //                           Mathf.FloorToInt(UnityEngine.Random.Range(0, TestSettings.Instance.OperationCount)));
    //        }
    //        for (int i = 0; i <= muscleNum; i++)
    //        {
    //            int node;
    //            int node2;

    //            if (i < nodeNum - 1)
    //            {
    //                node = i;
    //                node2 = i + 1;
    //            }
    //            else
    //            {
    //                node = UnityEngine.Random.Range(0, nodeNum);
    //                node2 = node;
    //                while (node == node2)
    //                {
    //                    node2 = UnityEngine.Random.Range(0, nodeNum);
    //                }
    //            }
    //            float s = 0.8f;
    //            if (i >= 10)
    //            {
    //                s *= 1.414f;
    //            }
    //            float length = UnityEngine.Random.Range(0.5f, 1.5f);
    //            creature.addRandMuscle(creature.gameObject.transform, node, node2);

    //        }
    //        //toStableConfiguration Stable configuration fix
    //        creature.SetInititationValues(y, 0, true, UnityEngine.Random.Range(40, 80), 1.0f);
    //    }
    //}



    public static Creature CreateCreature(Vector2 pos)
    {
        GameObject obj = new GameObject(); //Instantiates an empty gameobject
        Creature creature = obj.AddComponent<Creature>();
        creature.SetInititationValues(1, new List<Node>(), new List<Muscle>(), 0, true, 10, 0.8f);
        obj.transform.position = pos;

        return creature;
    }
    public static Creature CreateCreature(Vector2 pos, int ID, List<Node> nodes, List<Muscle> muscles, float d, bool alive, float timer, float mutability)
    {
        GameObject obj = new GameObject(); //Instantiates an empty gameobject
        Creature creature = obj.AddComponent<Creature>();
        creature.SetInititationValues(ID, nodes, muscles, d, alive, timer, mutability);
        obj.transform.position = pos;

        return creature;
    }

    public static string Name(int line)
    {
        string output;
        try
        {
            IEnumerable<char> oneLine = from c in Names[line] where Char.IsLetter(c) select c;
            output = string.Join("", oneLine.AsEnumerable()
              .Select(r => r.ToString().ToLower())
              .ToArray());
        }
        catch
        {
            output = "Glenn";
        }

        return output.First().ToString().ToUpper() + output.Substring(1);
    }

    protected CreatureHolder() { }
}
