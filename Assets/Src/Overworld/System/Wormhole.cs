using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;

public class Wormhole : Celestial
{
    public Wormhole(string name, Vector3 position, Random random) : base(name, position, random)
    {
        var choices = Resources.LoadAll<GameObject>("Celestials/Wormholes/");
        base.model = choices[random.Next(0, choices.Length)];
        base.type = PointOfInterestType.Wormhole;
    }

    public override void Interact()
    {
        if (!ProgressData.Evaluate(ProgressPoint.EnteredWormhole))
            DialogueUIManager.getInstance.DisplayDialogueEvent(EventDB.GetByID(1100));
        else
        {
            RuntimeData.GenerateNewSystem();
            SceneManager.LoadSceneAsync("Overworld");
        }
    }

    public override string GetTooltip()
    {
        return (ProgressData.Evaluate(ProgressPoint.EnteredWormhole) ? "" : "A scientific anomaly, perhaps worthy of a closer inspection.");
    }
}
