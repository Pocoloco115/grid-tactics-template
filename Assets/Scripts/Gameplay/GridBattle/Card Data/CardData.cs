using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Base")]
    [SerializeField] private string _cardName;
    [SerializeField] private int _health;
    [Header("Movement")]
    [SerializeField] private MoveType _moveType = MoveType.Cardinal;
    [SerializeField] private List<Vector2Int> _customOffsets = new List<Vector2Int>();

    [SerializeField] private int _moveRange = 1;

    public int MoveRange => _moveRange;
    public MoveType MoveType => _moveType;

    public IEnumerable<Vector2Int> GetMoveDirections()
    {
        if (_moveType == MoveType.Hex)
        {
            yield return new Vector2Int(1, 0);
            yield return new Vector2Int(-1, 0);
            yield return new Vector2Int(0, 1);
            yield return new Vector2Int(0, -1);
            yield return new Vector2Int(1, -1);
            yield return new Vector2Int(-1, 1);
            yield break;
        }

        yield return new Vector2Int(1, 0);
        yield return new Vector2Int(-1, 0);
        yield return new Vector2Int(0, 1);
        yield return new Vector2Int(0, -1);
    }

    public IEnumerable<Vector2Int> GetCustomOffsets()
    {
        foreach (var offset in _customOffsets)
        {
            yield return offset;
        }
    }
}

public enum MoveType
{
    Cardinal,
    Hex,
    Custom
}
