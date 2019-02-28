using UnityEngine;

public class Palette 
{
    Texture2D _land;
    Texture2D _water;

    public Texture2D land { get { return _land; } }
    public Texture2D water { get { return _water; } }

    public Palette(Color[] land, Color[] water)
    {
        _land = new Texture2D(land.Length, 1, TextureFormat.ARGB32, false);
        _water = new Texture2D(water.Length, 1, TextureFormat.ARGB32, false);

        for (int i = 0; i < land.Length; i++)
            _land.SetPixel(i, 0, land[i]);
        for (int i = 0; i < water.Length; i++)
            _water.SetPixel(i, 0, water[i]);

        _land.Apply();
        _water.Apply();
    }
}
