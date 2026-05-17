using System.Collections.Generic;
using UnityEngine;

public static class BattleSpawnUtility
{
    public static List<Vector2Int> GetSpawnSlots(GridManager grid, int row, int desiredCount)
    {
        List<Vector2Int> validCells = new List<Vector2Int>();

        if (grid == null || desiredCount <= 0)
        {
            return validCells;
        }

        for (int x = 0; x < grid.Width; x++)
        {
            Vector2Int cell = new Vector2Int(x, row);
            if (grid.IsInside(cell))
            {
                validCells.Add(cell);
            }
        }

        if (validCells.Count == 0)
        {
            return validCells;
        }

        List<Vector2Int> result = new List<Vector2Int>();
        int count = Mathf.Min(desiredCount, validCells.Count);
        int spacing = validCells.Count >= (count * 2 - 1) ? 2 : 1;
        int span = 1 + ((count - 1) * spacing);
        int start = Mathf.RoundToInt(((validCells.Count - 1) * 0.5f) - (((count - 1) * spacing) * 0.5f));
        int maxStart = Mathf.Max(0, validCells.Count - span);
        start = Mathf.Clamp(start, 0, maxStart);

        for (int i = 0; i < count; i++)
        {
            int index = start + (i * spacing);
            if (index < 0 || index >= validCells.Count)
            {
                break;
            }

            result.Add(validCells[index]);
        }

        return result;
    }
}