using UnityEngine;

public class Loader : MonoBehaviour
{
    void Start()
    {
        //fking annoying editor reseting shit
        Time.timeScale = 1f;

        OverworldManager.Initialize();
    }
}
