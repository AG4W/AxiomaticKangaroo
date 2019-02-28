using UnityEngine;

using Random = System.Random;
using System;
using System.Linq;

public static class ItemDB
{
    static bool _hasInitialized = false;

    static Random _random;
    static ShipComponentData[][] _items;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _random = new Random();
        _items = new ShipComponentData[Enum.GetNames(typeof(ShipComponentRarity)).Length][];

        for (int i = 0; i < _items.Length; i++)
            _items[i] = Resources.LoadAll<ShipComponentData>("Data/Parts/")
                .Where(scd => scd.rarity == (ShipComponentRarity)i)
                .ToArray();

        _hasInitialized = true;
    }

    public static ShipComponent GetRandom()
    {
        return _items
            .RandomItem()
            .RandomItem()
            .Instantiate();
    }
    public static ShipComponent GetRandom(ShipComponentRarity topRarity)
    {
        return _items[_random.Next(0, (int)topRarity)]
            .RandomItem()
            .Instantiate((ShipComponentRarity)_random.Next(0, Enum.GetNames(typeof(ShipComponentRarity)).Length));
    }
}
