using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;

public class Stargate : PointOfInterest
{
    int _jumpCost;

    StarSystem _system;

    public Stargate(string name, Vector3 position, Random random) : base(name, position, random)
    {
        base.type = PointOfInterestType.Wormhole;

        _jumpCost = DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, base.random.Next(2, 4));
    }

    //need to generate new system at instnatiate so it doesnt inf. loop.
    public override GameObject Instantiate()
    {
        GenerateSystem();
        return base.Instantiate();
    }

    public override void Interact()
    {
        if(PlayerData.fleet.GetVital(FleetVitalType.ProcessedFuel).current >= _jumpCost)
        {
            PlayerData.fleet.GetVital(FleetVitalType.ProcessedFuel).Update(-_jumpCost);
            RuntimeData.SetSystem(_system);
            SceneManager.LoadSceneAsync("Overworld");
        }
        else
            LogManager.getInstance.AddEntry("Not enough " + FleetVital.Format(FleetVitalType.ProcessedFuel) + "! " + _jumpCost + " units required!", 15f, EntryType.Combat);
    }

    public override string GetTooltip()
    {
        bool primitiveDataAvailable = OverworldManager.turnCount > DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, 5);
        bool advancedDataAvailable = OverworldManager.turnCount > DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, 8);

        string s = "";

        s += "Leads to [" + _system.star.name + "]:";
        s += "\n";

        s += primitiveDataAvailable ? "" : "Basic data will become available in " + (DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, 5) - OverworldManager.turnCount) + " turn(s).\n";
        s += advancedDataAvailable ? "" : "Advanced data will become available in " + (DifficultyUtils.Multiply(RuntimeData.save.data.difficulty, 8) - OverworldManager.turnCount) + " turn(s).\n";
        s += "\n";

        s += (primitiveDataAvailable ? _system.planets.Count.ToString() : "?") + " planet(s).\n";
        s += (primitiveDataAvailable ? _system.stargates.Count.ToString() : "?") + " stargate(s).\n";
        s += (primitiveDataAvailable ? _system.wormholes.Count.ToString() : "?") + " wormhole(s).\n";
        s += "\n";

        if (advancedDataAvailable)
        {
            s += "? fleets.\n";
            s += "\n";
        }

        s += "<color=" + (PlayerData.fleet.GetVital(FleetVitalType.ProcessedFuel).current >= _jumpCost ? "white" : "red") + ">Jump cost: " + _jumpCost + "</color> " + FleetVital.Format(FleetVitalType.ProcessedFuel);

        return s;
    }

    void GenerateSystem()
    {
        int seed = base.random.Next(0, int.MaxValue);

        while (seed == RuntimeData.system.seed)
            seed = base.random.Next(0, int.MaxValue);

        _system = new StarSystem(seed);
        _system.Generate();

        base.name = _system.star.name;
    }
}
