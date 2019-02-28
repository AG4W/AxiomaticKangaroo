using System.Runtime.Serialization;

[DataContract]
public class SaveData
{
    [DataMember]int _seed;

    [DataMember]Faction _player;
    [DataMember]Faction _enemy;

    [DataMember]Difficulty _difficulty;

    public int seed { get { return _seed; } }

    public Difficulty difficulty { get { return _difficulty; } }

    public Faction player { get { return _player; } }
    public Faction enemy { get { return _enemy; } }

    public SaveData(int seed)
    {
        _seed = seed;
        _player = new Faction();
        _enemy = new Faction();
    }
    public SaveData(int seed, Faction player, Faction enemy)
    {
        _seed = seed;
        _player = player;
        _enemy = enemy;
    }

    public void SetDifficulty(Difficulty d)
    {
        _difficulty = d;
    }
}
