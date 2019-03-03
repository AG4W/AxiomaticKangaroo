using UnityEngine;

using System;
using System.Collections.Generic;

public class Officer
{
    string _name;
    string _homeplanet;

    int _level;

    List<Trait> _traits;

    Sprite _portrait;
    Rank _rank;
    Gender _gender;

    Ship _assignment;

    public string name { get { return _name; } }
    public string homeplanet { get { return _homeplanet; } }

    public int level { get { return _level; } }

    public List<Trait> traits { get { return _traits; } }

    public Sprite portrait { get { return _portrait; } }
    public Rank rank { get { return _rank; } }

    public Ship assignment { get { return _assignment; } }

    public Officer()
    {
        _gender = (Gender)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Gender)).Length);
        _rank = Rank.Ensign;

        _name = NameGenerator.GetOfficerName(_gender);
        _homeplanet = NameGenerator.GetPOIName(PointOfInterestType.Planet);

        _portrait = ModelDB.GetPortrait();
        _traits = new List<Trait>();
        _traits.Add(TraitDB.GetRandom());
    }
    public Officer(string name, string homeplanet, Sprite portrait, Rank rank, Gender gender)
    {
        _name = name;
        _homeplanet = homeplanet;

        _portrait = portrait;
        _rank = rank;
        _gender = gender;
        _traits = new List<Trait>();
    }

    public void Assign(Ship ship)
    {
        _assignment = ship;
    }

    public void IncrementLevel()
    {
        if (_rank == (Rank)Enum.GetNames(typeof(Rank)).Length - 1)
            return;

        _level++;

        if (_level == 10)
        {
            Rank newRank = _rank++;

            if (LogManager.getInstance != null)
                LogManager.getInstance.AddEntry(_rank.ToString() + " " + _name + " has been promoted to: " + newRank.ToString());

            _rank = newRank;
            _level = 1;
        }
    }

    public void AddTrait(Trait t)
    {
        _traits.Add(t);

        if (LogManager.getInstance != null)
            LogManager.getInstance.AddEntry(_rank.ToString() + " " + _name + " has gained a trait: " + t.title + ".");

        OnTraitAdded?.Invoke(t);
    }
    public void RemoveTrait(Trait t)
    {
        _traits.Remove(t);

        if (LogManager.getInstance != null)
            LogManager.getInstance.AddEntry(_rank.ToString() + " " + _name + " has lost a trait: " + t.title + ".");

        OnTraitRemoved?.Invoke(t);
    }

    public override string ToString()
    {
        return
            _rank + " " + _name + "\n" +
            "Level: " + _level + "\n" +
            "Assignment: " + (_assignment == null ? "<color=green>Available</color>" : "<color=yellow>" + _assignment.name + "</color>");
    }
    public string GetSummary()
    {
        string s = "";

        s += _rank + " " + _name + "\n\n";
        s += "Level: " + _level + "\n";
        s += "Assignment: " + (_assignment == null ? "<color=green>Available</color>" : "<color=yellow>" + _assignment.name + "</color>\n");

        for (int i = 0; i < _traits.Count; i++)
            s += _traits[i].ToString() + "\n";

        return s;
    }

    public delegate void TraitEvent(Trait t);
    public static event TraitEvent OnTraitAdded;
    public static event TraitEvent OnTraitRemoved;
}
public enum Rank
{
    Ensign,
    Midshipsman,
    Lieutenant,
    Commodore,
    Captain,
}
public enum Gender
{
    Male,
    Female,
    Other
}
