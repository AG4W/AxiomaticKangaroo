using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;

public class GenericTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    Action _onEnter;
    Action _onLeftClick;
    Action _onMiddleClick;
    Action _onRightClick;
    Action _onExit;

    [Header("Sounds")]
    [SerializeField]ButtonSFX _mouseEnter = ButtonSFX.DefaultMouseEnter;
    [SerializeField]ButtonSFX _leftMouseDown = ButtonSFX.DefaultMouseDown;
    [SerializeField]ButtonSFX _scrollWheelDown = ButtonSFX.DefaultMouseDown;
    [SerializeField]ButtonSFX _rightMouseDown = ButtonSFX.DefaultMouseDown;
    [SerializeField]ButtonSFX _mouseExit = ButtonSFX.None;

    [Header("Visuals")]
    [SerializeField]bool _highlightOnEnterExit = true;
    [SerializeField]Color _highlightColor = new Color(.66f, 1f, 1f, 1f);

    [SerializeField]Image[] _images;
    [SerializeField]Text[] _texts;

    Color[] _originalImageColors;
    Color[] _originalTextColors;

    public void Initialize(Action onEnter, Action onLeftClick, Action onMiddleClick, Action onRightClick, Action onExit)
    {
        UpdateActions(onEnter, onLeftClick, onMiddleClick, onRightClick, onExit);

        if (_images == null)
            _images = new Image[0];

        _originalImageColors = new Color[_images.Length];

        for (int i = 0; i < _images.Length; i++)
            _originalImageColors[i] = _images[i].color;

        if (_texts == null)
            _texts = new Text[0];

        _originalTextColors = new Color[_texts.Length];

        for (int i = 0; i < _texts.Length; i++)
            _originalTextColors[i] = _texts[i].color;
    }
    public void UpdateActions(Action onEnter, Action onLeftClick, Action onMiddleClick, Action onRightClick, Action onExit)
    {
        _onEnter = onEnter;
        _onLeftClick = onLeftClick;
        _onMiddleClick = onMiddleClick;
        _onRightClick = onRightClick;
        _onExit = onExit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_mouseEnter != ButtonSFX.None)
            AudioManager.getInstance.PlayButtonSFX(_mouseEnter);
        if (_highlightOnEnterExit)
            HighlightUIElements();

        _onEnter?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (_leftMouseDown != ButtonSFX.None)
                    AudioManager.getInstance.PlayButtonSFX(_leftMouseDown);

                _onLeftClick?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                if (_scrollWheelDown != ButtonSFX.None)
                    AudioManager.getInstance.PlayButtonSFX(_scrollWheelDown);

                _onMiddleClick?.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                if (_rightMouseDown != ButtonSFX.None)
                    AudioManager.getInstance.PlayButtonSFX(_rightMouseDown);

                _onRightClick?.Invoke();
                break;
        }

        eventData.Use();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_mouseExit != ButtonSFX.None)
            AudioManager.getInstance.PlayButtonSFX(_mouseExit);
        if(_highlightOnEnterExit)
            ResetUIElements();

        _onExit?.Invoke();
    }

    public void HighlightOnEnterExit(bool highlightOnEnterExit)
    {
        _highlightOnEnterExit = highlightOnEnterExit;
    }
    public void OverrideUIElementColor(Color color)
    {
        for (int i = 0; i < _images.Length; i++)
            _images[i].color = color;
        for (int i = 0; i < _texts.Length; i++)
            _texts[i].color = color;
    }
    void HighlightUIElements()
    {
        for (int i = 0; i < _images.Length; i++)
            _images[i].color = _highlightColor;
        for (int i = 0; i < _texts.Length; i++)
            _texts[i].color = _highlightColor;
    }
    void ResetUIElements()
    {
        for (int i = 0; i < _images.Length; i++)
            _images[i].color = _originalImageColors[i];

        for (int i = 0; i < _texts.Length; i++)
            _texts[i].color = _originalTextColors[i];
    }
}
