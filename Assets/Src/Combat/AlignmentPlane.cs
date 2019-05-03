using UnityEngine;

using System.Collections.Generic;

public static class AlignmentPlane
{
    static float _size;

    static GameObject _plane;
    static bool _isActive;

    public static float y { get { return _plane.transform.position.y; } }
    public static bool isActive { get { return _isActive; } }

    public static void Initialize()
    {
        _plane = GameObject.Find("alignmentPlane");
        _size = RuntimeData.localMapData.size * 2;

        Event.Subscribe(ActionEvent.ToggleAlignmentPlane, (object[] args) => Toggle());

        SetSize();
    }

    public static void Set(float y)
    {
        _plane.transform.position = new Vector3(0, y, 0);
    }
    public static void SetToAverage(List<ShipEntity> entities)
    {
        float y = 0;

        for (int i = 0; i < entities.Count; i++)
            y += entities[i].transform.position.y;

        Vector3 np = _plane.transform.position;
        np.y = Mathf.Clamp(entities.Count > 0 ? y / entities.Count : 0, -_size, _size);

        _plane.transform.position = np;
    }
    public static void Shift(float y)
    {
        _plane.transform.position += new Vector3(0, y, 0);

        Vector3 position = _plane.transform.position;
        position.y = Mathf.Clamp(position.y, -_size, _size);

        _plane.transform.position = position;
    }

    static void Toggle()
    {
        for (int i = 0; i < _plane.transform.childCount; i++)
        {
            Renderer r = _plane.transform.GetChild(i).GetComponent<Renderer>();
            r.enabled = !r.enabled;
        }

        _isActive = _plane.transform.GetChild(0).gameObject.activeSelf;
    }
    public static void SetActive(bool status)
    {
        for (int i = 0; i < _plane.transform.childCount; i++)
        {
            Renderer r = _plane.transform.GetChild(i).GetComponent<Renderer>();
            r.enabled = status;
        }

        _isActive = status;
    }

    static void SetSize()
    {
        _plane.transform.GetChild(0).localScale = new Vector3(_size, _size, 0);
        _plane.transform.GetChild(1).localScale = new Vector3(_size, _size, 0);
    }
}
