using UnityEngine;
using UnityEngine.Tilemaps;

public class RectGridManager : GridManager
{
    [Header("Rect Grid")]
    [SerializeField] private Tilemap _rectTilemap;
    [SerializeField] private TileBase _rectTile;
    [SerializeField] private Color _rectBaseColor = Color.white;
    [SerializeField] private Color _rectOffsetColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private int _rectTileSortingOrder = 0;

    protected override void GenerateGrid()
    {
        if (_rectTilemap == null || _rectTile == null)
        {
            return;
        }

        _rectTilemap.ClearAllTiles();
        PositionCameraAtCenter(new Vector2Int(Width / 2, Height / 2));

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                bool isOffset = (x + y) % 2 == 1;
                Color tileColor = isOffset ? _rectOffsetColor : _rectBaseColor;

                _rectTilemap.SetTile(cell, _rectTile);
                _rectTilemap.SetTileFlags(cell, TileFlags.None);
                _rectTilemap.SetColor(cell, tileColor);
            }
        }

        TilemapRenderer renderer = _rectTilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = _rectTileSortingOrder;
        }
    }

    public override Vector3 GetGridWorldPosition(Vector2Int gridPos)
    {
        if (_rectTilemap == null)
        {
            return new Vector3(gridPos.x, gridPos.y, 0f);
        }

        return _rectTilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    public override Vector2Int GetGridPositionFromWorld(Vector2 worldPosition)
    {
        if (_rectTilemap == null)
        {
            return Vector2Int.zero;
        }

        Vector3Int cell = _rectTilemap.WorldToCell(worldPosition);
        return new Vector2Int(cell.x, cell.y);
    }
}
