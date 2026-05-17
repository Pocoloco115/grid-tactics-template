using System.Collections.Generic;
using UnityEngine;

public abstract class GridManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] protected int _width = 5;
    [SerializeField] protected int _height = 5;
    [SerializeField] protected Vector2 _origin = Vector2.zero;
    [SerializeField] protected Camera _camera;

    [Header("Highlight")]
    [SerializeField] private GameObject _moveHighlightPrefab;

    [Header("Rendering")]
    [SerializeField] private float _cardZ = -0.1f;
    [SerializeField] private float _highlightZ = -0.05f;
    [SerializeField] private int _cardSortingOrder = 10;
    [SerializeField] private int _highlightSortingOrder = 5;

    [Header("Cards")]
    [SerializeField] protected CardBehaviour _cardPrefab;

    public int Width
    {
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }

    public int CardSortingOrder
    {
        get
        {
            return _cardSortingOrder;
        }
    }

    protected readonly Dictionary<Vector2Int, CardBehaviour> _cards = new Dictionary<Vector2Int, CardBehaviour>();
    protected readonly HashSet<Vector2Int> _highlighted = new HashSet<Vector2Int>();

    protected virtual void Start()
    {
        GenerateGrid();
    }

    protected abstract void GenerateGrid();
    public abstract Vector3 GetGridWorldPosition(Vector2Int gridPos);
    public abstract Vector2Int GetGridPositionFromWorld(Vector2 worldPosition);

    protected void PositionCameraAtCenter(Vector2Int centerCell)
    {
        if (_camera == null)
        {
            return;
        }

        Vector3 centerPos = GetGridWorldPosition(centerCell);
        _camera.transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
    }

    public virtual bool IsInside(Vector2Int p)
    {
        if (p.x < 0)
        {
            return false;
        }

        if (p.y < 0)
        {
            return false;
        }

        if (p.x >= _width)
        {
            return false;
        }

        if (p.y >= _height)
        {
            return false;
        }

        return true;
    }

    public bool IsOccupied(Vector2Int p)
    {
        return _cards.ContainsKey(p);
    }

    public CardBehaviour GetCard(Vector2Int p)
    {
        _cards.TryGetValue(p, out CardBehaviour card);
        return card;
    }

    public CardBehaviour SpawnCard(CardData data, Vector2Int pos, Team team)
    {
        if (data == null)
        {
            return null;
        }

        if (!IsInside(pos))
        {
            return null;
        }

        if (IsOccupied(pos))
        {
            return null;
        }

        CardBehaviour card = Instantiate(_cardPrefab, GetCardWorldPosition(pos), Quaternion.identity, transform);
        card.Setup(data, pos, this, team);
        card.ApplySorting(_cardSortingOrder);
        _cards[pos] = card;
        return card;
    }

    public virtual List<Vector2Int> GetHexNeighbors(Vector2Int pos)
    {
        Debug.LogWarning("Default GetHexNeighbors used, override in HexGridManager");
        return new List<Vector2Int>();
    }

    public List<Vector2Int> GetValidMoves(CardBehaviour card)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (card == null)
        {
            return result;
        }

        if (card.Data.MoveType == MoveType.Hex)
        {
            HexGridManager hexGrid = this as HexGridManager;
            if (hexGrid == null)
            {
                return result;
            }

            Queue<(Vector2Int pos, int dist)> toCheck = new Queue<(Vector2Int pos, int dist)>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            visited.Add(card.GridPos);
            toCheck.Enqueue((card.GridPos, 0));

            while (toCheck.Count > 0)
            {
                (Vector2Int current, int dist) = toCheck.Dequeue();

                if (dist >= card.Data.MoveRange)
                {
                    continue;
                }

                List<Vector2Int> neighbors = hexGrid.GetHexNeighbors(current);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Vector2Int n = neighbors[i];

                    if (visited.Contains(n))
                    {
                        continue;
                    }

                    visited.Add(n);

                    if (IsOccupied(n))
                    {
                        continue;
                    }

                    result.Add(n);
                    toCheck.Enqueue((n, dist + 1));
                }
            }

            return result;
        }

        if (card.Data.MoveType == MoveType.Custom)
        {
            HexGridManager hexGrid = this as HexGridManager;

            foreach (Vector2Int offset in card.Data.GetCustomOffsets())
            {
                Vector2Int target;

                if (hexGrid != null)
                {
                    target = hexGrid.GetOffsetFromAxial(hexGrid.GetAxialFromOffset(card.GridPos) + offset);
                }
                else
                {
                    target = card.GridPos + offset;
                }

                if (!IsInside(target))
                {
                    continue;
                }

                if (IsOccupied(target))
                {
                    continue;
                }

                result.Add(target);
            }

            return result;
        }

        foreach (Vector2Int direction in card.Data.GetMoveDirections())
        {
            for (int i = 1; i <= card.Data.MoveRange; i++)
            {
                Vector2Int target = card.GridPos + (direction * i);

                if (!IsInside(target))
                {
                    break;
                }

                if (IsOccupied(target))
                {
                    break;
                }

                result.Add(target);
            }
        }

        return result;
    }

    public bool TryMove(CardBehaviour card, Vector2Int dest)
    {
        if (card == null)
        {
            return false;
        }

        if (!IsInside(dest))
        {
            return false;
        }

        if (IsOccupied(dest))
        {
            return false;
        }

        List<Vector2Int> valid = GetValidMoves(card);
        if (!valid.Contains(dest))
        {
            return false;
        }

        _cards.Remove(card.GridPos);
        card.MoveTo(dest);
        _cards[dest] = card;
        return true;
    }

    public void ClearMoveHighlights()
    {
        foreach (Vector2Int p in _highlighted)
        {
            Transform highlight = transform.Find($"MoveHighlight {p.x} {p.y}");
            if (highlight != null)
            {
                Destroy(highlight.gameObject);
            }
        }

        _highlighted.Clear();
    }

    public void ShowMoveHighlights(IEnumerable<Vector2Int> positions)
    {
        ClearMoveHighlights();

        foreach (Vector2Int p in positions)
        {
            if (!IsInside(p))
            {
                continue;
            }

            if (_moveHighlightPrefab == null)
            {
                continue;
            }

            Vector3 worldPos = GetCardWorldPosition(p);
            worldPos.z = _highlightZ;

            GameObject highlightInstance = Instantiate(_moveHighlightPrefab, worldPos, Quaternion.identity, transform);
            highlightInstance.name = $"MoveHighlight {p.x} {p.y}";

            SpriteRenderer[] renderers = highlightInstance.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in renderers)
            {
                sr.sortingOrder = _highlightSortingOrder;
            }

            _highlighted.Add(p);
        }
    }

    public bool TryGetGridPositionAtWorld(Vector2 world, out Vector2Int gridPos)
    {
        gridPos = GetGridPositionFromWorld(world);
        return IsInside(gridPos);
    }

    public Vector3 GetCardWorldPosition(Vector2Int gridPos)
    {
        Vector3 pos = GetGridWorldPosition(gridPos);
        pos.z = _cardZ;
        return pos;
    }
}