using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;
    [SerializeField] private Vector2 _cellSize = Vector2.one;
    [SerializeField] private Vector2 _origin = Vector2.zero;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Camera _camera;

    [Header("Rendering")]
    [SerializeField] private float _tileZ = 0f;
    [SerializeField] private float _cardZ = -0.1f;
    [SerializeField] private int _tileSortingOrder = 0;
    [SerializeField] private int _cardSortingOrder = 10;

    [Header("Cards")]
    [SerializeField] private CardBehaviour _cardPrefab;
    public int Width => _width;
    public int Height => _height;
    public int TileSortingOrder => _tileSortingOrder;
    public int CardSortingOrder => _cardSortingOrder;

    private readonly Dictionary<Vector2Int, Tile> _tiles = new();
    private readonly Dictionary<Vector2Int, CardBehaviour> _cards = new();
    private readonly HashSet<Vector2Int> _highlighted = new();
    private void Start()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        _tiles.Clear();
        if(_camera == null) return;
        _camera.transform.position = new Vector3(
            _origin.x + ((_width - 1) * _cellSize.x * 0.5f),
            _origin.y + ((_height - 1) * _cellSize.y * 0.5f),
            -10f);

         for(int x = 0; x < _width; x++)
         {
             for(int y = 0; y < _height; y++)
             {
                Vector2Int pos = new Vector2Int(x, y);
                     Tile tile = Instantiate(_tilePrefab, GetTileWorldPosition(pos),
                Quaternion.identity, transform);
                tile.name = $"Tile {pos.x} {pos.y}";

                bool isOffset = (x + y) % 2 == 1;
                tile.Init(pos, isOffset, this);
                tile.ApplySorting(_tileSortingOrder);

                _tiles[pos] = tile;
             }
         }
    }
    public bool IsInside(Vector2Int p) => p.x >= 0 && p.y >= 0 && p.x < _width 
    && p.y < _height;
    public bool IsOccupied(Vector2Int p) => _cards.ContainsKey(p);
    public CardBehaviour GetCard(Vector2Int p)
    {
        _cards.TryGetValue(p, out CardBehaviour card);
        return card;
    }
    public CardBehaviour SpawnCard(CardData data, Vector2Int pos, Team team)
    {
        if(data == null) return null;
        if(!IsInside(pos) || IsOccupied(pos)) return null;

        var card = Instantiate(_cardPrefab, GetCardWorldPosition(pos),
        Quaternion.identity, transform);
        card.Setup(data, pos, this, team);
        card.ApplySorting(_cardSortingOrder);
        _cards[pos] = card;
        return card;
    }
    public List<Vector2Int> GetValidMoves(CardBehaviour card)
    {
        var result = new List<Vector2Int>();
        if(card == null) return result;

        foreach(var offset in card.Data.GetMoveOffsets())
        {
            var target = card.GridPos + offset;
            if (!IsInside(target)) continue;
            if (IsOccupied(target)) continue;
            result.Add(target);
        }
        return result;
    }
    public bool TryMove(CardBehaviour card, Vector2Int dest)
    {
        if (card == null) return false;
        if (!IsInside(dest)) return false;
        if (IsOccupied(dest)) return false;

        bool allowed = false;
        foreach (var offset in card.Data.GetMoveOffsets())
        {
            if (card.GridPos + offset == dest)
            {
                allowed = true;
                break;
            }
        }
        if (!allowed) return false;

        _cards.Remove(card.GridPos);
        card.MoveTo(dest);
        _cards[dest] = card;

        return true;
    }

    public void ClearMoveHighlights()
    {
        foreach (var p in _highlighted)
        {
            if (_tiles.TryGetValue(p, out var tile))
            {
                tile.SetMoveHighlight(false);
            }
        }
        _highlighted.Clear();
    }

    public void ShowMoveHighlights(IEnumerable<Vector2Int> positions)
    {
        ClearMoveHighlights();

        foreach (var p in positions)
        {
            if (!_tiles.TryGetValue(p, out var tile)) continue;
            tile.SetMoveHighlight(true);
            _highlighted.Add(p);
        }
    }
    public bool TryGetTileAtWorld(Vector2 world, out Tile tile)
    {
        tile = null;
        var hit = Physics2D.Raycast(world, Vector2.zero);
        if (hit.collider == null) return false;
        return hit.collider.TryGetComponent(out tile);
    }

    public Vector3 GetTileWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(
            _origin.x + (gridPos.x * _cellSize.x),
            _origin.y + (gridPos.y * _cellSize.y),
            _tileZ);
    }

    public Vector3 GetCardWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(
            _origin.x + (gridPos.x * _cellSize.x),
            _origin.y + (gridPos.y * _cellSize.y),
            _cardZ);
    }
}