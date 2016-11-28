using UnityEngine;
using System.Collections;

/// <summary>
/// Improves upon Singleton by encouraging good coding practices 
/// http://wiki.unity3d.com/index.php/Toolbox 
/// </summary>
public class FishSettings : Singleton<FishSettings> 
{
    protected FishSettings() { } // guarantee this will be always a singleton only - can't use the constructor!

    /// <summary>
    /// The Unity prefab the fishes will use
    /// </summary>
    public GameObject FishPrefab;

    public float FishHorizontalPadding = 0;

    public float FishVerticalPadding = 0;

    /// <summary>
    /// Max number of fishes ever active on the screen
    /// </summary>
    public int MaxFish = 1;

    /// <summary>
    /// Spawnradius from Shark. If null it will be set to 50f.
    /// </summary>
    public float SpawnRadius = 50;

    public float FishSpeed = 5f;

    public float SharkSpeed = 20f;

    public float TurnSpeed = 180f;

    public float JumpSpeed = 20f;

    public int TimeScale = 25;

    public int Generation = 1;
    bool IsWithinBounds = true;
    public float SensorRange = 10;


    void Awake()
    {
        // Your initialization code here

    }

    // (optional) allow runtime registration of global objects
    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}

