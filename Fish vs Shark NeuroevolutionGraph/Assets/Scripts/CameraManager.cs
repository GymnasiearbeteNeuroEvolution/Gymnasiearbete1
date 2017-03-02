using UnityEngine;
using System.Collections;
using System;
#pragma strict
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
    /// <summary>
    /// Checks if a Vector3 in the world is within the bounds of the camera.
    /// </summary>
    public static bool IsWithinCameraBounds(Vector3 position, float xPadding, float yPadding)
    {
        //Checks for the vertical axis
        if ((position.y - yPadding) > TopLeftCorner.y || (position.y + yPadding) < BottomLeftCorner.y)
            return false;
        //Checks for the horizontal axis
        else if ((position.x - xPadding) > TopRightCorner.x || (position.x + xPadding) < TopLeftCorner.x)
            return false;
        else
            return true;

    }
}
