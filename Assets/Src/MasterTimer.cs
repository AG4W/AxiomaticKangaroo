using UnityEngine;

using System;
using System.Collections;

public class MasterTimer : MonoBehaviour
{
    static MasterTimer _instance;
    public static MasterTimer getInstance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    public void Request(float duration, Action onElapsed)
    {
        StartCoroutine(Timer(duration, onElapsed));
    }
    IEnumerator Timer(float duration, Action onElapsed)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        onElapsed();
    }
}
