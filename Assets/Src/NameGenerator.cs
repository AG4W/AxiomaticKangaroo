using UnityEngine;

using System.Linq;

public static class NameGenerator
{
    static string[] _descriptives = new string[]
    {
        "Agile",
        "Solemn",
        "Independent",
        "Lucky",
        "Vengeful",
        "Solar",
        "Celestial",
        "Astral",
        "Steadfast",
        "Joyful",
        "Intrepid",
        "Mournful",
        "Thundering",
        "Zealous",
    };
    static string[] _suffixes = new string[]
    {
        "Shadow",
        "Sovereign",
        "Spirit",
        "Phantom",
        "Errant",
        "Queen",
        "King",
        "Legend",
        "Star",
        "Monarch",
        "Hand",
        "Fist",
        "Pathfinder",
        "Spectre",
        "Jewel",
        "Crown",
        "Explorer",
        "Bastion",
        "Pioneer",
        "Pillar",
        "Trace",
        "Prophet",
        "Bishop",
        "Knight",
        "Lord",
        "Emperor",
        "Empress",
        "Wanderer",
        "Sword",
        "Spear",
        "Shield",
        "Bulwark",
        "Hammer",
    };
    static string[] _prefixes = new string[]
    {
        "Truth",
        "Intent",
        "Fire",
        "Ice",
        "Amber",
        "Vengeance",
        "Fury",
        "Joy",
        "Faith",
        "Zeal",
        "Courage",
        "Despair",
        "Anger",
        "Light",
        "Darkness",
        "Unity",
        "Communion",
        "Hope",
        "Spring",
        "Summer",
        "Autumn",
        "Winter"
    };
    static string[] _midfixes = new string[]
    {
        "of",
    };

    static string[] _stars = new string[]
    {
        "Anen",
        "Aran",
        "Aton",
        "Kroni",
        "Nu Xiu",
        "Phalguni",
        "Cerberi",
        "Humbaba",
        "Phofa",
        "Shen Xiu",
        "Cassiopeiae",
        "Herschelii",
        "Latruhines",
        "Dinkanope",
        "Vallion",
        "New Iapetus",
        "Jakri's Stand",
        "F'Hani",
        "New Titan",
        "Ortin",
        "Da-Yu",
        "A'Sara",
    };

    #region Factions
    static string[][] _factionTypes;
    static string[] _planets = new string[]
    {
        "Askya",
        "New Persia",
        "New Mena",
        "Artemis Prime",
        "New Quebec",
        "New Ganymede",
        "Phaeton",
        "Artemis",
        "New Earth",
        "New Mars",
        "Philammon",
        "Ao-Shun",
        "New Vaalbara",
        "Churquark Sp'Vasi",
        "Uron",
        "Leto",
        "Telmun",
        "Alnon",
        "Miniea",
        "Kobol",
        "Arrakis",
        "Kobol",
        "Triton",
        "Earth",
        "Mars",
        "Sagittarion",
        "Apollo",
        "Arrakis",
        "Caprica",
    };
    #endregion
    #region Officers
    static string[][] _firstnames = new string[][]
    {
        new string[]
        {
            "Axel",
            "Albin",
            "Johan",
            "Erik",
            "Torbjörn",
            "Christophorus",
            "Kippar",
            "Loy",
            "Jone",
            "Gerri",
            "Gunar",
            "Edan",
            "Griz",
            "Hadlee"
        },
        new string[]
        {
            "Veronica",
            "Jacqueline",
            "Loy",
            "Jone",
        }
    };
    static string[] _surnames = new string[]
    {
        "Gustafsson",
        "Fagernes",
        "Andersson",
        "Qvarnström",
        "Fernström",
    };
    #endregion
    
    static bool _hasInitialized = false;

    public static void Initialize()
    {
        if (_hasInitialized)
            return;
        else
            _hasInitialized = true;

        TextAsset[] types = Resources.LoadAll<TextAsset>("Text/Government Namelists/");

        _factionTypes = new string[types.Length][];

        for (int i = 0; i < _factionTypes.Length; i++)
            _factionTypes[i] = types
                .First(t => t.name == ((FactionType)i)
                .ToString().ToLower())
                .text
                .Split('\n');
    }

    public static string GetPOIName(PointOfInterestType type)
    {
        switch (type)
        {
            case PointOfInterestType.Star:
                return _stars.RandomItem();
            case PointOfInterestType.Planet:
                return _planets.RandomItem();
            default:
                return "RANDOM NAME FAILED!";
        }
    }
    public static string GetShipName()
    {
        int r = Random.Range(0, 2);
        if (r == 1)
            return (_descriptives[Random.Range(0, _descriptives.Length)] + " " + _suffixes[Random.Range(0, _suffixes.Length)]);
        else
            return (_suffixes[Random.Range(0, _suffixes.Length)] + " " + _midfixes[Random.Range(0, _midfixes.Length)] + " " + _prefixes[Random.Range(0, _prefixes.Length)]);
    }

    public static string GetFactionType(FactionType type)
    {
        return _factionTypes[(int)type].RandomItem();
    }

    public static string GetOfficerName(Gender gender)
    {
        string name = "";

        if (gender == Gender.Other)
            name += _firstnames.RandomItem().RandomItem();
        else
            name += _firstnames[(int)gender].RandomItem();

        name += " " + _surnames.RandomItem();

        return name;
    }
}