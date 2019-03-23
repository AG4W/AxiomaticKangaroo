using UnityEngine;
using UnityEngine.UI;

public class CrewUIManager : MonoBehaviour
{
    static CrewUIManager _instance;
    public static CrewUIManager getInstance { get { return _instance; } }

    [SerializeField]Transform _list;

    [SerializeField]GameObject _window;
    [SerializeField]GameObject _listItem;

    public bool isOpen { get { return _window.activeSelf; } }

    void Awake()
    {
        _instance = this;

        Close();
    }

    public void Open()
    {
        CreateCrewList();

        _window.SetActive(true);
    }
    public void Close()
    {
        _window.SetActive(false);
    }

    void CreateCrewList()
    {
        CleanUp();

        for (int i = 0; i < PlayerData.officers.Count; i++)
            CreateCrewItem(PlayerData.officers[i]);
    }
    void CreateCrewItem(Officer o)
    {
        GameObject g = Instantiate(_listItem, _list);

        g.transform.Find("portrait").GetComponent<Image>().sprite = o.portrait;
        g.transform.Find("text").GetComponent<Text>().text = o.ToString();

        g.GetComponent<GenericTooltipHandler>().Initialize(
            () => TooltipManager.getInstance.OpenTooltip(o.ToString(), Input.mousePosition),
            null,
            null,
            null,
            () => TooltipManager.getInstance.CloseTooltip());
    }

    void CleanUp()
    {
        for (int i = 0; i < _list.childCount; i++)
            Destroy(_list.GetChild(i).gameObject);
    }
}
