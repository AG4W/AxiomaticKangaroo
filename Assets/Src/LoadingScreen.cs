using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    static LoadingScreen _instance;
    public static LoadingScreen getInstance { get { return _instance; } }

    [SerializeField]GameObject _loadingScreen;
    [SerializeField]Image _screen;

    Color _opaque;
    Color _transparent;

    void Awake()
    {
        if (_instance != null)
            Destroy(this.gameObject);
        else
        {
            _instance = this;

            _opaque = new Color(_screen.color.r, _screen.color.g, _screen.color.b, 1f);
            _transparent = new Color(_screen.color.r, _screen.color.g, _screen.color.b, 0f);

            _loadingScreen.SetActive(false);

            //SceneManager.activeSceneChanged += Open;
            //SceneManager.sceneLoaded += Close;

            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Open(Scene current, Scene next)
    {
        StartCoroutine(FadeIn(1f));
    }
    void Close(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeOut(1f));
    }

    IEnumerator FadeIn(float time)
    {
        _screen.color = _transparent;
        _loadingScreen.SetActive(true);

        float t = 0f;

        while (t <= time)
        {
            t += Time.deltaTime;
            _screen.color = Color.Lerp(_transparent, _opaque, t / time);
            yield return null;
        }
    }
    IEnumerator FadeOut(float time)
    {
        float t = 0f;

        while (t <= time)
        {
            t += Time.deltaTime;
            _screen.color = Color.Lerp(_opaque, _transparent, t / time);
            yield return null;
        }

        _loadingScreen.SetActive(false);
    }
}
