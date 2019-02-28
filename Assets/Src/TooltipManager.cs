using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    static TooltipManager _instance;
    public static TooltipManager getInstance { get { return _instance; } }

    [SerializeField]GameObject _tooltip;

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        _tooltip.SetActive(false);
    }

    public void OpenTooltip(string text, Vector3 position)
    {
        if (text == null || text.Length == 0)
            return;

        _tooltip.transform.Find("description").GetComponent<Text>().text = text;
        _tooltip.transform.position = position;
        _tooltip.SetActive(true);
    }
    public void CloseTooltip()
    {
        _tooltip.SetActive(false);
    }
}
