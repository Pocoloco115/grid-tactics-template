using System;
using System.Collections.Generic;
using UnityEngine;

public class UIActionPanel : MonoBehaviour
{
    [SerializeField] private List<UIActionItem> _items = new List<UIActionItem>();

    public UIActionItem SelectedItem { get; private set; }

    public event Action<UIActionItem> SelectionChanged;

    private void Awake()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            UIActionItem item = _items[i];
            if (item == null)
            {
                continue;
            }

            item.Clicked += OnItemClicked;
            item.SetSelected(false);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            UIActionItem item = _items[i];
            if (item == null)
            {
                continue;
            }

            item.Clicked -= OnItemClicked;
        }
    }

    private void OnItemClicked(UIActionItem item)
    {
        SetSelected(item);
    }

    public void SetSelected(UIActionItem item)
    {
        if (item == SelectedItem)
        {
            ClearSelection(); 
            return;
        }

        if (SelectedItem != null)
        {
            SelectedItem.SetSelected(false);
        }

        SelectedItem = item;

        if (SelectedItem != null)
        {
            SelectedItem.SetSelected(true);
        }

        SelectionChanged?.Invoke(SelectedItem);
    }

    public void ClearSelection()
    {
        if (SelectedItem != null)
        {
            SelectedItem.SetSelected(false);
        }

        SelectedItem = null;
        SelectionChanged?.Invoke(null);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ClearSelection();
    }

    public void Hide()
    {
        ClearSelection();
        gameObject.SetActive(false);
    }
}