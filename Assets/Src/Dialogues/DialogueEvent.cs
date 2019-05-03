using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Events/Event")]
public class DialogueEvent : ScriptableObject
{
    [SerializeField]int _id = 1000;
    [Range(0.01f, 1f)][SerializeField]float _probability = 1f;

    [Tooltip("" +
        "LISTPLAYERFLEET\n" +
        "FORMATVITAL_FLEETVITALNAME\n"
        )]
    [Header("Mouseover me for command list")]
    [TextArea(5, 15)][SerializeField]string _body;

    [SerializeField]List<DialogueOption> _options;

    public int id { get { return _id; } }
    public float probability { get { return _probability; } }

    public string body { get { return _body; } }

    public List<DialogueOption> options { get { return _options; } }
}
