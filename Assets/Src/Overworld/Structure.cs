using UnityEngine;

using Random = System.Random;

public class Structure : PointOfInterest
{
    DialogueEvent _event;
    Module _module;

    public Structure(string name, Cell cell, Random random, Module module) : base(name, cell, random)
    {
        base.type = PointOfInterestType.Structure;

        _module = module;
        _event = EventDB.GetByName(name);
    }

    public override void Interact()
    {
        //base.Interact();
        DialogueUIManager.getInstance.DisplayDialogueEvent(_event);
    }

    public static string FormatName(Module module)
    {
        switch (module)
        {
            case Module.Refinery:
                return "Orbital Refinery";
            case Module.ManufacturingPlant:
                return "Orbital Manifacturing Plant";
            case Module.TradingPost:
                return "Orbital Trading Post";
            case Module.None:
            default:
                return "Orbital Structure";
        }
    }
}
public enum Module
{
    Refinery,
    ManufacturingPlant,
    TradingPost,
    None
}
