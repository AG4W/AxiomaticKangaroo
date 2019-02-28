using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]float _speed = 1f;
    [SerializeField]Vector3 _axis;

    [SerializeField]bool _randomize = true;

    void Start()
    {
        if (_randomize)
        {
            _axis = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f));
        }

        _axis = _axis.normalized;
    }
    void Update()
    {
        this.transform.Rotate(_axis, _speed * Time.deltaTime);
    }
}
