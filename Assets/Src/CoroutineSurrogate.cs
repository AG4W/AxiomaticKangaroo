using UnityEngine;

public class CoroutineSurrogate : MonoBehaviour
{
    public static CoroutineSurrogate getInstance { get; private set; }

    void Awake()
    {
        getInstance = this;
    }
}
