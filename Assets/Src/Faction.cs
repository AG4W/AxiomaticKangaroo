using UnityEngine;

using System.Runtime.Serialization;

[DataContract]
public class Faction
{
    [DataMember]string _name;
    [DataMember]string _homePlanet;

    [DataMember]FactionType _type;

    public string name { get { return _name; } }
    public string homePlanet { get { return _homePlanet; } }

    public FactionType type { get { return _type; } }

    public Faction()
    {
        _type = (FactionType)Random.Range(0, System.Enum.GetNames(typeof(FactionType)).Length);
        _homePlanet = NameGenerator.GetPOIName(PointOfInterestType.Planet);
        _name = _homePlanet + " " + NameGenerator.GetFactionType(_type);
    }
}
public enum FactionType
{
    Monarchy,
    Republic,
    CityState,
    Technocracy,
    Theocracy
}
