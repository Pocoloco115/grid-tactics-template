using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _offsetColor;

    [Header("Highlight")]
    [SerializeField] private GameObject _hoverHighlight;
    [SerializeField] private GameObject _moveHighlight;

    public Vector2Int GridPos { get; private set; }
    private GridManager _grid;
    public void Init(Vector2Int gridPos, bool isOffset, GridManager grid)
    {
        _spriteRenderer.color = isOffset ? _offsetColor : _baseColor;
        GridPos = gridPos;
        _grid = grid;

        if(_hoverHighlight) _hoverHighlight.SetActive(false);
        if(_moveHighlight) _moveHighlight.SetActive(false);
    }
    public void SetMoveHighlight(bool active)
    {
        if(_moveHighlight) _moveHighlight.SetActive(active);
    }

    public void ApplySorting(int baseOrder)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = baseOrder;
        }

        SetHighlightSorting(_hoverHighlight, baseOrder + 1);
        SetHighlightSorting(_moveHighlight, baseOrder + 2);
    }

    private static void SetHighlightSorting(GameObject target, int order)
    {
        if (target == null) return;

        var renderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            sr.sortingOrder = order;
        }
    }

    private void OnMouseEnter()
    {
        if(_hoverHighlight) _hoverHighlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        if(_hoverHighlight) _hoverHighlight.SetActive(false);
    }
}

