//using UnityEngine;
//using System.Collections;
//using System.Linq;

///// <summary>
///// Finds The fish closest to it and tries to catch it 
///// </summary>
//public class SharkController : MonoBehaviour {
        
    

//	// Use this for initialization
//	void Start ()
//    {


//	}
	
//	// Update is called once per frame
//	void Update ()
//    {
//        Vector3 currentPosition = transform.position;
//        if(GameObject.FindGameObjectWithTag("Fish") != null)
//        {
//            var closestGameObject = GameObject.FindGameObjectsWithTag("Fish")
//               .Select(go => new { go = go, position = go.transform.position })
//                 .Aggregate((current, next) =>
//                     (current.position - currentPosition).sqrMagnitude <
//                         (next.position - currentPosition).sqrMagnitude
//                             ? current : next).go;
//            var dir = Vector3.Normalize(closestGameObject.transform.position - currentPosition);

//            transform.position += dir * Time.deltaTime * FishSettings.Instance.SharkSpeed;
//        }
    

//    }
//}
