using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class GraphMaker : MonoBehaviour
{
    private Dictionary<int, float> ks;
    static string[] Fitness;
    private string output;
    private float value;
    private bool running;

    void Start()
    {

    }
    void Update()
    {
    }
    public void CreateGraph(string directory)
    {
        try
        {
            Fitness = File.ReadAllLines(directory);
        }
        catch (Exception e)
        {
            print(e);
        }
        ks = new Dictionary<int, float>();


        for (int i = 0; i < Fitness.Length; i++)
        {
            IEnumerable<char> oneLine = from c in Fitness[i] select c;
            output = string.Join("", oneLine.AsEnumerable()
              .Select(r => r.ToString().ToLower())
              .ToArray());
            output = output.Split(':')[2];
            var integers = from c in output where Char.IsDigit(c) || c == '.' select c;

            output = new string(integers.ToArray());
            value = float.Parse(output, CultureInfo.InvariantCulture.NumberFormat);

            ks[i] = value;
        }
        float highest = 0;
        for(int i = 1; i < ks.Count; i++)
        {
            if(highest < ks[i])
            {
                highest = ks[i];
            }
            if(i == 100 || i == 200 || i == 300 || i == 400 || i == 500 || i == 600 || i == 700 || i == 800 || i == 900 || i == 1000 || i == 1100)
            {
               
                GameObject obj = Instantiate(TestSettings.Instance.Bar, new Vector3(10, 0, 0), Quaternion.identity) as GameObject;
                obj.transform.SetParent(gameObject.transform);
                print(highest);
                obj.transform.position = new Vector3((i/5)-20, highest/2, 0);

                obj.transform.localScale = new Vector3(5, highest, 5);
            }
        }
        //for (int i = 1; i < ks.Count -1; i++)
        //{
        //    GameObject obj = Instantiate(TestSettings.Instance.MusclePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //    obj.transform.SetParent(gameObject.transform);

        //    obj.transform.position = new Vector3(i, ks[i], 0);
        //    Vector2 diff;
        //    diff = new Vector3((i+1), ks[i+1]) - new Vector3((i) , ks[i]);
            
        //    float hyp = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.y, 2));
        //    float rot = 0;
        //    obj.transform.localScale = new Vector3(1f, hyp / 2,1f);
        //    if (diff.x != 0)
        //        rot = Mathf.Atan(diff.y / diff.x);
        //    obj.transform.position = (new Vector3(i/5, ks[i],-500) + new Vector3((i + 1)/5 , ks[i + 1])) / 2;
        //    int asd;
        //    if (rot > (3 * Mathf.PI / 2))
        //        asd = -90;
        //    else
        //        asd = 90;
        //    obj.transform.localRotation = Quaternion.Euler(0, 0, asd + (rot * 180) / Mathf.PI);
        //}
        Camera.main.transform.rotation = Quaternion.Euler(0, 180, 0);
        Camera.main.orthographicSize = 60;
        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        gameObject.transform.position = new Vector3((ks.Count / 5) / 2, -10, -500);
        for(int i = 0; i < 8; i++)
        {
            int y = i * 10;
            GameObject text = Instantiate(TestSettings.Instance.TextPrefab, new Vector3(110, i * 10 -7, -230), Quaternion.Euler(0,180,0)) as GameObject;
            var Textm = text.GetComponent<TextMesh>();
            Textm.text = y.ToString();
            Textm.fontSize = 100;
        }
        for (int i = 1; i <= (Fitness.Length/100); i++)
        {
            int y = i * 100;
            GameObject text = Instantiate(TestSettings.Instance.TextPrefab, new Vector3(115 - i * 20, -15, -230), Quaternion.Euler(0, 180, 0)) as GameObject;
            var Textm = text.GetComponent<TextMesh>();
            Textm.text = y.ToString();
            Textm.fontSize = 100;
        }
    }
	

}
