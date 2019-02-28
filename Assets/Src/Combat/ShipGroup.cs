using UnityEngine;

using System.Collections.Generic;

public class ShipGroup 
{
    List<ShipEntity> _ships;
    public List<ShipEntity> ships { get { return _ships; } }

    public ShipGroup(List<ShipEntity> ships)
    {
        _ships = ships;
    }

    public Vector3 GetCenter()
    {
        if (_ships.Count == 1)
            return _ships[0].transform.position;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        for (int i = 0; i < _ships.Count; i++)
        {
            x += _ships[i].transform.position.x;
            y += _ships[i].transform.position.y;
            z += _ships[i].transform.position.z;
        }

        x = x / _ships.Count;
        y = y / _ships.Count;
        z = z / _ships.Count;

        return new Vector3(x, y, z);
    }

    public bool Contains(ShipEntity se)
    {
        return _ships.IndexOf(se) != -1;
    }
    public void Remove(ShipEntity se)
    {
        _ships.Remove(se);
    }
}
