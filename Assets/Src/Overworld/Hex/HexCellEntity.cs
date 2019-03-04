using UnityEngine;

public class HexCellEntity : MonoBehaviour
{
    void Start()
    {
        this.GetComponentInChildren<HexMesh>().Triangulate(this);
    }
}
