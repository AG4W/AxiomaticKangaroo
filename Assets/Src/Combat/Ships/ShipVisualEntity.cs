using UnityEngine;
using UnityEngine.UI;

public class ShipVisualEntity : MonoBehaviour
{
    ShipEntity _entity;

    GameObject _graphics;
    GameObject _destructionVFX;

    [SerializeField]Text[] _nameDecals;
    [SerializeField]AnimatedPart[] _parts;

    public void Initialize(ShipEntity entity, GameObject destructionVFX)
    {
        _entity = entity;
        _destructionVFX = destructionVFX;

        _graphics = this.transform.Find("graphics").gameObject;
        _graphics.SetActive(entity.isDiscovered);

        for (int i = 0; i < _nameDecals.Length; i++)
            _nameDecals[i].text = _entity.name;

        entity.OnDiscovered += OnDiscovered;
        entity.OnDestroyed += OnDestroyed;
    }

    void Update()
    {
        AnimateParts();
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        AnimateParts();
    }
#endif

    void OnDiscovered(bool hasBeenDiscovered)
    {
        _graphics.SetActive(hasBeenDiscovered);
    }
    void OnDestroyed(ShipEntity s)
    {
        CreateDestructionVFX();
    }

    void AnimateParts()
    {
        for (int i = 0; i < _parts.Length; i++)
            _parts[i].model.transform.localEulerAngles += _parts[i].rotation * (_parts[i].rotationSpeed * Time.deltaTime);
    }

    void CreateDestructionVFX()
    {
        Transform root = this.transform.Find("graphics");
        Transform[] children = root.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetParent(null);

            Rigidbody rb = children[i].gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.mass = 1;
            rb.drag = .75f;
            rb.angularDrag = .75f;
            rb.AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(5f, 10f), ForceMode.Impulse);
        }

        Destroy(Instantiate(_destructionVFX, this.transform.position, Random.rotation, null), 3f);
    }
}
[System.Serializable]
public struct AnimatedPart
{
    [SerializeField]public GameObject model;

    [SerializeField]public Vector3 rotation;
    [SerializeField]public float rotationSpeed;
}
