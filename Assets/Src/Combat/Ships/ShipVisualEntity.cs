using UnityEngine;

public class ShipVisualEntity : MonoBehaviour
{
    ShipEntity _entity;

    GameObject _graphics;
    GameObject _destructionVFX;

    public void Initialize(ShipEntity entity, GameObject destructionVFX)
    {
        _entity = entity;
        _destructionVFX = destructionVFX;

        _graphics = this.transform.Find("graphics").gameObject;
        _graphics.SetActive(entity.isDiscovered);

        entity.OnDiscovered += OnDiscovered;
        entity.OnDestroyed += OnDestroyed;
    }

    void OnDiscovered(bool hasBeenDiscovered)
    {
        _graphics.SetActive(hasBeenDiscovered);
    }
    void OnDestroyed(ShipEntity s)
    {
        CreateDestructionVFX();
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
