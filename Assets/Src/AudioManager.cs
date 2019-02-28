using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using System.Collections;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager getInstance { get { return _instance; } }

    [Header("Interface")]
    [SerializeField]AudioSource _ui;
    [SerializeField]AudioClip[] _buttonSFX;

    [Header("OST")]
    [SerializeField]AudioSource _ost;
    [SerializeField]AudioClip[] _ostThemes;

    [Header("Local Map Ambience")]
    [SerializeField]AudioSource _ambience;
    [SerializeField]AudioClip[] _ambiences;

    [Header("Master")]
    [SerializeField]AudioMixer _master;

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
        {
            _instance = this;

            Settings.OnVolumeChanged += Set;
            SceneManager.activeSceneChanged += OnSceneChanged;

            DontDestroyOnLoad(this.gameObject);
        }
    }


    public void PlayButtonSFX(ButtonSFX buttonSFX)
    {
        _ui.clip = _buttonSFX[(int)buttonSFX];
        _ui.pitch = Random.Range(.95f, 1.05f);
        _ui.Play();
    }

    public void PlayOST(OSTTheme theme, float fadeTime = 4f)
    {
        _ost.clip = _ostThemes[(int)theme];
        _ost.Play();

        Fade(_ost, fadeTime, true);
    }
    public void StopOST(float fadeTime = 4f)
    {
        Fade(_ost, fadeTime, false);
    }

    void OnSceneChanged(Scene current, Scene next)
    {
        _ost.Stop();

        if (next.name == "LocalMap")
        {
            _ambience.clip = _ambiences.RandomItem();
            _ambience.Play();
        }
        else
            _ambience.Stop();
    }

    void Set(VolumeSource vs, float volume)
    {
        _master.SetFloat(vs.ToString(), (-40f * (1 - volume)));
    }

    void Fade(AudioSource source, float fade, bool fadeIn)
    {
        StartCoroutine(FadeAudioSource(source, fade, fadeIn));
    }
    IEnumerator FadeAudioSource(AudioSource source, float fade, bool fadeIn)
    {
        float t = 0f;
        float l = source.volume;

        //if (fadeIn)
        //    source.Play();

        while (t <= fade)
        {
            t += Time.deltaTime;

            if (!fadeIn)
                source.volume = Mathf.Lerp(l, 0f, t / fade);
            else
                source.volume = Mathf.Lerp(0f, l, t / fade);

            yield return null;
        }

        if (!fadeIn)
            source.Stop();
    }
}
public enum VolumeSource
{
    Master,
    Interface,
    OST,
    Game
}
public enum ButtonSFX
{
    DefaultMouseEnter,
    DefaultMouseDown,
    DefaultMouseExit,
    None,
}
public enum OSTTheme
{
    MainMenu
}
