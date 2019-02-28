using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GraphicalSettingsManager : MonoBehaviour
{
    static GraphicalSettingsManager _instance;
    public static GraphicalSettingsManager getInstance { get { return _instance; } }

    [SerializeField]PostProcessProfile[] _profiles;

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
        {
            _instance = this;

            Settings.OnGraphicSettingChanged += ToggleGraphicSetting;
            DontDestroyOnLoad(this.transform);
        }
    }

    void ToggleGraphicSetting(GraphicSetting ppe, bool status)
    {
        //for (int i = 0; i < _profiles.Length; i++)
        //{
        //    switch (ppe)
        //    {
        //        case GraphicSetting.DepthOfField:
        //            var dof = _profiles[i].GetSetting<DepthOfField>();

        //            if(dof != null)
        //                dof.enabled.value = status;
        //            break;
        //        case GraphicSetting.MotionBlur:
        //            var mb = _profiles[i].GetSetting<MotionBlur>();

        //            if (mb != null)
        //                mb.enabled.value = status;
        //            break;
        //    }
        //}
    }
}
public enum GraphicSetting
{
    DepthOfField,
    MotionBlur
}
