using UnityEngine;

using Random = System.Random;
using System.Linq;

public static class ItemDB
{
    static bool _hasInitialized = false;

    static Random _random;
    static ShipComponentData[] _items;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _random = new Random();
        _items = Resources.LoadAll<ShipComponentData>("Data/Parts/");
        _hasInitialized = true;
    }

    public static ShipComponent GetRandom()
    {
        return _items
            .RandomItem()
            .Instantiate();
    }
    public static ShipComponent GetRandomCapRarity(ShipComponentRarity rarity)
    {
        return _items
            .Where(i => i.rarityCap <= rarity)
            .ToArray()
            .RandomItem()
            .Instantiate();
    }
}
