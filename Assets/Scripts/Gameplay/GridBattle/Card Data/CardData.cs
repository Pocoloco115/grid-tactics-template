using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Base")]
    [SerializeField] private string _cardName;
    [SerializeField] private int _health;
    [Header("Movement")]
    [SerializeField] private MoveType _moveType = MoveType.CardinalRange;
    [SerializeField] private int _cardinalRange = 1;
    [SerializeField] private List<Vector2Int> _moveOffsets = new List<Vector2Int>()
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    public IEnumerable<Vector2Int> GetMoveOffsets()
    {
        if (_moveType == MoveType.Offsets)
        {
            foreach (var offset in _moveOffsets)
            {
                yield return offset;
            }
            yield break;
        }
        for(int i = 1; i <= _cardinalRange; i++)
        {
            yield return new Vector2Int(i, 0);
            yield return new Vector2Int(-i, 0);
            yield return new Vector2Int(0, i);
            yield return new Vector2Int(0, -i);
        }
    }

}
public enum MoveType
{
    Offsets,
    CardinalRange
}
