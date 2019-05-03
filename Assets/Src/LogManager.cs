using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class LogManager : MonoBehaviour
{
    public static LogManager getInstance { get; private set; }

    [SerializeField]GameObject _log;
    [SerializeField]GameObject _logItem;

    void Awake()
    {
        if (getInstance != null)
            Destroy(this.transform.root.gameObject);
        else
        {
            getInstance = this;
            DontDestroyOnLoad(this.transform.root);
        }
    }
    public void AddEntry(string text, float lifetime = 15f, EntryType type = EntryType.Default)
    {
        GameObject g = Instantiate(_logItem, _log.transform);
        Text t = g.GetComponentInChildren<Text>();

        t.color = GetColorFromType(type);
        t.text = text;
        
        StartCoroutine(UpdateEntryAsync(g, t, lifetime));
    }

    Color GetColorFromType(EntryType type)
    {
        switch (type)
        {
            case EntryType.Combat:
                return Color.red;
            case EntryType.Harvesting:
                return Color.yellow;
            case EntryType.Dialogue:
                return Color.cyan;
            case EntryType.Default:
                return Color.white;
            default:
                return Color.magenta;
        }
    }

    IEnumerator UpdateEntryAsync(GameObject entry, Text text, float lifetime)
    {
        //Color c = text.color;
        //float a = c.a;
        float t = 0f;

        while (t <= lifetime)
        {
            t += Time.deltaTime;
            //c.a = Mathf.Lerp(a, 0f, t / lifetime);
            //text.color = c;
            yield return null;
        }

        Destroy(entry);
    }
}
public enum EntryType
{
    Combat,
    Harvesting,
    Dialogue,
    Default
}
