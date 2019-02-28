using UnityEngine;

public class CoroutineSurrogate : MonoBehaviour
{
    static CoroutineSurrogate _instance;
    public static CoroutineSurrogate getInstance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }
}
