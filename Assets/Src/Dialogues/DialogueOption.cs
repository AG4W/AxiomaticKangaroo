using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [SerializeField]string _conditions;
    [TextArea(3, 10)][SerializeField]string _text;
    [TextArea(3, 10)][SerializeField]string _tooltip;
    [TextArea(3, 10)][SerializeField]string _commands;

    [SerializeField]bool _closeAfterSelect = true;

    public string conditions { get { return _conditions; } }
    public string text { get { return _text; } }
    public string tooltip { get { return _tooltip; } }
    public string commands { get { return _commands; } }

    public bool closeAfterSelect { get { return _closeAfterSelect; } }
}
