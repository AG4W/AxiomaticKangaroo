using UnityEngine;

[CreateAssetMenu(menuName = "Art/Palettes/Palette")]
public class PaletteData : ScriptableObject
{
    [SerializeField]Color[] _land = new Color[] { Color.black, Color.blue, Color.cyan, Color.green, Color.grey, Color.white };
    [SerializeField]Color[] _water = new Color[] { Color.gray, Color.blue, Color.cyan };

    public Palette Instantiate()
    {
        return new Palette(_land, _water);
    }
}