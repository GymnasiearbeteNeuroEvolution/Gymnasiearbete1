using UnityEngine;
using System.Collections;

public class Manager : Singleton<Manager>
{
    protected Manager() { } // guarantee this will always be a singleton only - can't use the constructor!

    int currentScore;

    public void AdjustScore(int num)
    {
        currentScore += num;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 100), "Num = " + currentScore);
    }
}
