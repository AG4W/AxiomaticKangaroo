using UnityEngine;

using System;
using System.IO;

public static class Settings
{
    const float DEFAULT_VOLUME = .4f;

    static float[] _volumes;
    public static float GetVolume(VolumeSource v)
    {
        return _volumes[(int)v];
    }
    public static void SetVolume(VolumeSource v, float f)
    {
        _volumes[(int)v] = f;

        OnVolumeChanged?.Invoke(v, f);
    }

    public static void SetGraphicSetting(GraphicSetting ppe, bool status)
    {
        OnGraphicSettingChanged?.Invoke(ppe, status);
    }

    public static void SetResolution(Resolution r)
    {
        Screen.SetResolution(r.width, r.height, true, r.refreshRate);
    }
    public static void SetFullScreenMode(FullScreenMode fsmode)
    {
        Screen.fullScreenMode = fsmode;
    }
    public static void SetAnisotropicFiltering(AnisotropicFiltering mode)
    {
        QualitySettings.anisotropicFiltering = mode;
    }
    public static void SetAntiAliasing(int aa)
    {
        QualitySettings.antiAliasing = aa;
    }
    public static void SetVerticalSync(int mode)
    {
        QualitySettings.vSyncCount = mode;
    }
    public static void SetTextureQuality(int sizeLimit)
    {
        QualitySettings.masterTextureLimit = sizeLimit;
    }
    public static void SetShadowQuality(ShadowResolution sr)
    {
        QualitySettings.shadowResolution = sr;
    }

    public static void Load(string path = null)
    {
        if (path == null)
            path = Application.persistentDataPath + "settings.ini";

        _volumes = new float[Enum.GetNames(typeof(VolumeSource)).Length];

        //if settings are saved, load them
        if (File.Exists(path))
        {
            string[] settings = File.ReadAllLines(path);

            //load volumes
            for (int i = 0; i < _volumes.Length; i++)
                SetVolume((VolumeSource)i, float.Parse(settings[i + 1].Split('=')[1]));
        }
        //else load defaults
        else
        {
            for (int i = 0; i < _volumes.Length; i++)
                SetVolume((VolumeSource)i, DEFAULT_VOLUME);
        }
    }
    public static void Save(string path = null)
    {
        if (path == null)
            path = Application.persistentDataPath + "settings.ini";

        //string[] settings = new string[]
        //{
        //    "#volume",
        //    "master=-30f",
        //    "interface=-30f",
        //    "ost=-30f",
        //    "game=-30f"
        //};
    }
    
    public delegate void VolumeChangedEvent(VolumeSource vs, float v);
    public static event VolumeChangedEvent OnVolumeChanged;

    public delegate void GraphicChangedEvent(GraphicSetting gs, bool s);
    public static event GraphicChangedEvent OnGraphicSettingChanged;
}