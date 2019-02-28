using UnityEngine;

using System;

public class Officer
{
    string _name;
    string _homeplanet;

    int _level;

    Sprite _portrait;
    Rank _rank;
    Gender _gender;

    Ship _assignment;

    public string name { get { return _name; } }
    public string homeplanet { get { return _homeplanet; } }

    public int level { get { return _level; } }

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
    }
    public Officer(string name, string homeplanet, Sprite portrait, Rank rank, Gender gender)
    {
        _name = name;
        _homeplanet = homeplanet;

        _portrait = portrait;
        _rank = rank;
        _gender = gender;
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

    public override string ToString()
    {
        return
            _rank + " " + _name + "\n" +
            "Current Assignment: " + (_assignment == null ? "<color=green>Available</color>" : "Commanding the '<i><color=yellow>" + _assignment.name + "</color></i>'") + "\n" +
            "Level: " + _level + "\n" + 
            "Homeplanet: " + _homeplanet + "\n";
    }
}
public enum Rank
{
    Ensign,
    Midshipsman,
    Lieutenant,
    Commodore,
    Captain
}
public enum Gender
{
    Male,
    Female,
    Other
}
