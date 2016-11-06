using UnityEngine;
using System.Collections;
/// <summary>
/// Improves upon Singleton by encouraging good coding practices 
/// http://wiki.unity3d.com/index.php/Toolbox 
/// </summary>
public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public string myGlobalVar = "whatever";


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
