using UnityEngine;
using System.Collections;
using System;

public class CameraManager : MonoBehaviour
{
    public static Vector3 TopLeftCorner
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane)); }
    }
    public static Vector3 TopRightCorner
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)); }
    }
    public static Vector3 BottomLeftCorner
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)); }
    }
    public static Vector3 BottomRightCorner
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane)); }
    }

    void Start () {
    }

    // Update is called once per frame
    void Update () {
	}
}
