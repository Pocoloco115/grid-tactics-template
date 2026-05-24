using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIActionItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Refs")]
    [SerializeField] private GameObject _hoverPanel;
    [SerializeField] private GameObject _selectedOverlay;
    [SerializeField] private CanvasGroup _selectedOverlayCanvasGroup;

    [Header("Selected Overlay Fade")]
    [SerializeField] private float _pulseSpeed = 2.0f;
    [SerializeField] private float _minAlpha = 0.15f;
    [SerializeField] private float _maxAlpha = 0.65f;
    [SerializeField] private float _fadeInTime = 0.08f;

    public event Action<UIActionItem> Clicked;

    private bool _isSelected;
    private float _currentAlpha;

    private void Awake()
    {
        if (_hoverPanel != null)
        {
            _hoverPanel.SetActive(false);
        }

        if (_selectedOverlay != null)
        {
            _selectedOverlay.SetActive(false);
        }

        if (_selectedOverlayCanvasGroup == null && _selectedOverlay != null)
        {
            _selectedOverlayCanvasGroup = _selectedOverlay.GetComponent<CanvasGroup>();
        }

        if (_selectedOverlayCanvasGroup != null)
        {
            _selectedOverlayCanvasGroup.alpha = 0f;
        }

        _currentAlpha = 0f;
    }

    private void Update()
    {
        if (_isSelected)
        {
            UpdateSelectedOverlayPulse();
        }
    }

    private void UpdateSelectedOverlayPulse()
    {
        if (_selectedOverlayCanvasGroup == null)
        {
            return;
        }

        float t = (Mathf.Sin(Time.unscaledTime * _pulseSpeed) + 1f) * 0.5f;
        float target = Mathf.Lerp(_minAlpha, _maxAlpha, t);

        float k = 1f;
        if (_fadeInTime > 0f)
        {
            k = 1f - Mathf.Exp(-Time.unscaledDeltaTime / _fadeInTime);
        }

        _currentAlpha = Mathf.Lerp(_currentAlpha, target, k);
        _selectedOverlayCanvasGroup.alpha = _currentAlpha;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        if (_selectedOverlay == null)
        {
            return;
        }

        if (selected)
        {
            _selectedOverlay.SetActive(true);

            if (_selectedOverlayCanvasGroup != null)
            {
                _currentAlpha = 0f;
                _selectedOverlayCanvasGroup.alpha = 0f;
            }
        }
        else
        {
            if (_selectedOverlayCanvasGroup != null)
            {
                _selectedOverlayCanvasGroup.alpha = 0f;
            }

            _currentAlpha = 0f;
            _selectedOverlay.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverPanel != null)
        {
            _hoverPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverPanel != null)
        {
            _hoverPanel.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}