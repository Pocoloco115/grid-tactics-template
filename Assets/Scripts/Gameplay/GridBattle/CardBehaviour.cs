using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private CardData _cardData;
    [SerializeField] private GridManager _grid;
    public CardData Data
    {
        get => _cardData;
        private set => _cardData = value;
    }
    public Vector2Int GridPos { get; private set; }
    public Team Team { get; private set; }

    public void Setup(CardData data, Vector2Int startPos, GridManager grid,
    Team team)
    {
        Data = data;
        _grid = grid;
        Team = team;
        SetGridPosition(startPos);
    }
    public void SetGridPosition(Vector2Int newPos)
    {
        GridPos = newPos;
        if (_grid != null)
        {
            transform.position = _grid.GetCardWorldPosition(GridPos);
        }
        else
        {
            transform.position = new Vector3(GridPos.x, GridPos.y, 0);
        }
    }

    public void ApplySorting(int sortingOrder)
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            sr.sortingOrder = sortingOrder;
        }
    }

    public void MoveTo(Vector2Int dest)
    {
        SetGridPosition(dest);
    }
}
public enum Team
{
    Player,
    Enemy
}
