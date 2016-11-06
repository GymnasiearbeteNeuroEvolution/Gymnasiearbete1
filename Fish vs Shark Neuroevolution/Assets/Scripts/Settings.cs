using UnityEngine;
using System.Collections;

public static class Settings
{
    /// <summary>
    /// Max number of fishes ever active on the screen
    /// </summary>
    public static int MaxFish;
    /// <summary>
    /// Spawnradius from Shark. If null it will be set to 50f.
    /// </summary>
    public static float? SpawnRadius;

    // Use this for initialization
    private static void Start ()
    {
        if (SpawnRadius == null)
            SpawnRadius = 50f;
        
	}
}
