using UnityEngine;

public class Rarity
{
    float _modifier;
    Color _color;
    ShipComponentRarity _rarity;

    public float modifier { get { return _modifier; } }
    public Color color { get { return _color; } }
    public ShipComponentRarity rarity { get { return _rarity; } }

    public Rarity(float modifier, Color color, ShipComponentRarity rarity)
    {
        _modifier = modifier;
        _color = color;
        _rarity = rarity;
    }
}
