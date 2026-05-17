using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridManager : GridManager
{
    [Header("Hex Grid")]
    [SerializeField] private Tilemap _hexTilemap;
    [SerializeField] private TileBase _hexTile;
    [SerializeField] private Color _hexBaseColor = Color.white;
    [SerializeField] private Color _hexOffsetColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private int _hexTileSortingOrder = 0;

    private readonly HashSet<Vector2Int> _validCells = new HashSet<Vector2Int>();

    protected override void GenerateGrid()
    {
        if (_hexTilemap == null || _hexTile == null)
        {
            return;
        }

        _validCells.Clear();
        _hexTilemap.ClearAllTiles();
        PositionCameraAtCenter(new Vector2Int(Width / 2, Height / 2));

        int centerRow = Height / 2;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int rowLength = Width - Mathf.Abs(centerRow - y);
                int startX = (Width - rowLength) / 2;

                if (x < startX || x >= startX + rowLength)
                {
                    continue;
                }

                Vector3Int cell = new Vector3Int(x, y, 0);
                bool isOffset = (y & 1) == 1;
                Color tileColor = isOffset ? _hexOffsetColor : _hexBaseColor;

                _hexTilemap.SetTile(cell, _hexTile);
                _hexTilemap.SetTileFlags(cell, TileFlags.None);
                _hexTilemap.SetColor(cell, tileColor);
                _validCells.Add(new Vector2Int(x, y));
            }
        }

        TilemapRenderer renderer = _hexTilemap.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = _hexTileSortingOrder;
        }
    }

    public override Vector3 GetGridWorldPosition(Vector2Int gridPos)
    {
        if (_hexTilemap == null)
        {
            return new Vector3(gridPos.x, gridPos.y, 0f);
        }

        return _hexTilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    public override Vector2Int GetGridPositionFromWorld(Vector2 worldPosition)
    {
        if (_hexTilemap == null)
        {
            return Vector2Int.zero;
        }

        Vector3Int cell = _hexTilemap.WorldToCell(worldPosition);
        Vector2Int gridPos = new Vector2Int(cell.x, cell.y);

        if (_validCells.Contains(gridPos))
        {
            return gridPos;
        }

        return Vector2Int.zero;
    }

    public override bool IsInside(Vector2Int p)
    {
        return _validCells.Contains(p);
    }

    public Vector2Int GetAxialFromOffset(Vector2Int offset)
    {
        int q = offset.x - ((offset.y - (offset.y & 1)) / 2);
        int r = offset.y;
        return new Vector2Int(q, r);
    }

    public Vector2Int GetOffsetFromAxial(Vector2Int axial)
    {
        int x = axial.x + ((axial.y - (axial.y & 1)) / 2);
        int y = axial.y;
        return new Vector2Int(x, y);
    }

    public override List<Vector2Int> GetHexNeighbors(Vector2Int pos)
    {
        bool isOdd = (pos.y & 1) == 1;

        Vector2Int[] directionsEven = new Vector2Int[]
        {
            new Vector2Int(+1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, +1),
            new Vector2Int(0, +1),
        };

        Vector2Int[] directionsOdd = new Vector2Int[]
        {
            new Vector2Int(+1, 0),
            new Vector2Int(+1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, +1),
            new Vector2Int(+1, +1)
        };

        Vector2Int[] dirs = isOdd ? directionsOdd : directionsEven;

        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var dir in dirs)
        {
            Vector2Int neighbor = pos + dir;
            if (IsInside(neighbor))
            {
                result.Add(neighbor);
            }
        }
        return result;
    }
}