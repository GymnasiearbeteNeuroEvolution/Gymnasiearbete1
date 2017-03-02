/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishManager : MonoBehaviour
{
    private List<GameObject> fishList;
    private List<GameObject> fishListToBeRemoved;
    private List<Vector2> fishStartPos;
	private GameObject shark;
    public GUIStyle style;



    // Use this for initialization
    void Start ()
    {
        fishList = new List<GameObject>();
		shark = GameObject.FindGameObjectWithTag ("Shark");
        fishStartPos = GetVector2FromRadius(shark.transform.position, FishSettings.Instance.SpawnRadius, FishSettings.Instance.MaxFish);
        //CreateFish();
	}
	void CreateFish()
    {       
		for (int i = 0; i < FishSettings.Instance.MaxFish; i++)
        {
            GameObject fish = (GameObject)Instantiate(FishSettings.Instance.FishPrefab, new Vector3(fishStartPos[i].x, fishStartPos[i].y), Quaternion.identity); //Instantiate a new fish
            fish.transform.GetOrAddComponent<Fish>(); //Add the "Fish" script to it.
            fishList.Add(fish);
        }
        fishListToBeRemoved = fishList;
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
        //GameObject Shark = GameObject.FindGameObjectWithTag("Shark");
        List <Vector2> Vector2FromRadius= new List<Vector2>();
		float radiansPerFish = (Mathf.PI * 2) / amount;
        for (int i = 0; i < amount; i++)
        {
            float angle = radiansPerFish * i;
            
            Vector2 newPos = fromPos;
			newPos += new Vector2(Mathf.Cos(angle)* radius,Mathf.Sin(angle) * radius);
            Vector2FromRadius.Add(newPos);
        }
		return Vector2FromRadius;
    }

    void OnGUI()
    {
        for (int i = 0; i < fishList.Count; i++)
        {
            GameObject fish = fishList[i];
            //double fitness = System.Math.Round((float)fish.transform.GetOrAddComponent<Fish>().Fitness, 2); //Gets the fitenss value and rounds it to two decimals points
            //GUI.Label(new Rect(10, 10 + 50 * i, 100, 50), "Fitness nr " + i + ": " + fitness, style);
        }
    }
    void Update()
    {
        for (int i = 0; i < fishList.Count; i++) 
        {
            GameObject fish = fishList[i];
            fish.transform.name = "Fish: " + i;
            if (fish.transform.GetOrAddComponent<Fish>().IsHit)
            {
                fishListToBeRemoved.Remove(fish);
                fish.SetActive(false);
            }
        } 
    }
}
*/