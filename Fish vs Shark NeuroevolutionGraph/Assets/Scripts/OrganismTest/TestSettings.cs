using UnityEngine;
using System.Collections;

/// <summary>
/// Improves upon Singleton by encouraging good coding practices 
/// http://wiki.unity3d.com/index.php/Toolbox 
/// </summary>
public class TestSettings : Singleton<TestSettings> 
{
    protected TestSettings() { } // guarantee this will be always a singleton only - can't use the constructor!

    /// <summary>
    /// The Unity prefab the fishes will use
    /// </summary>
    public GameObject FishPrefab;

    public GameObject NodePrefab;

    public GameObject CreaturePrefab;

    public GameObject MusclePrefab;

    public GameObject TextPrefab;

    public float ZValue;

    public float FishHorizontalPadding = 0;

    public int[] OperationAxons = { 0, 0, 0, 0, 2, 2, 2, 2, 2, 1, 1, 0 };

    public int OperationCount = 12;

    public float FishVerticalPadding = 0;

    /// <summary>
    /// Sets the max/min values for 
    /// </summary>
    public float NodeMinRad = 2, NodeMaxRad = 4;

    public string ProjectName = "OrganismExperiment";

    /// <summary>
    /// Max number of fishes ever active on the screen
    /// </summary>
    public int MaxFish = 1;

    public float FishSpeed = 5f;

    public float TurnSpeed = 180f;

    public float JumpSpeed = 20f;

    public int Generation = 1;

    public float SensorRange = 10;

    public float airfriction = 0.95f; //1-friction

    public int MaxNodes = 6;

    public int MaxMuscles = 12;

    // the smallest and largest lenght the muscle can achieve
    public float lMax = 8, lMin = 2;

    public float MaxNodeSpeed = 2;

    public GameObject Bar;

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


