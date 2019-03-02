using UnityEngine;

using Random = System.Random;
using System.Linq;

public static class RarityDB
{
    static Rarity[] _rarities;
    static Random _random;

    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;

        _random = new Random();

        _rarities = new Rarity[]
        {
            new Rarity(1f, new Color(1f, 1f, 1f), ShipComponentRarity.Common),
            new Rarity(1.125f, new Color(.75f, 1f, .75f), ShipComponentRarity.Exotic),
            new Rarity(1.25f, new Color(.75f, .75f, 1f), ShipComponentRarity.Mastercraft),
            new Rarity(1.5f, new Color(1f, .75f, 0f), ShipComponentRarity.Artifact)
        };

        _hasInitialized = true;
    }

    public static Rarity Get(ShipComponentRarity rarity)
    {
        return _rarities.FirstOrDefault(r => r.rarity == rarity);
    }
    public static Rarity GetWeighted(ShipComponentRarity bot, ShipComponentRarity cap)
    {
        Rarity prospect;
        float r = _random.Next(0, 100);

        if (r <= 2)
            prospect = Get(ShipComponentRarity.Artifact);
        else if (r <= 7.5)
            prospect = Get(ShipComponentRarity.Mastercraft);
        else if(r <= 20)
            prospect = Get(ShipComponentRarity.Exotic);
        else
            prospect = Get(ShipComponentRarity.Common);

        if (prospect.rarity <= bot)
            return Get(bot);
        else if (prospect.rarity >= cap)
            return Get(cap);

        return prospect;
    }
}
