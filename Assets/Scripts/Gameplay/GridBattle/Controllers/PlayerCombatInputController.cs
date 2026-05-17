using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatInputController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GridManager _grid;
    [SerializeField] private TurnManager _turns;

    private CardBehaviour _selected;

    private InputAction _click;

    public void SetGrid(GridManager grid)
    {
        _grid = grid;
    }

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _click = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
        _click.performed += OnClick;
    }

    private void OnEnable()
    {
        _click.Enable();
    }

    private void OnDisable()
    {
        _click.Disable();
    }

    private void OnDestroy()
    {
        _click.performed -= OnClick;
        _click.Dispose();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (!_turns.IsPlayersTurn)
        {
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 world3 = _camera.ScreenToWorldPoint(screenPos);
        Vector2 world2D = new(world3.x, world3.y);

        var hit = Physics2D.Raycast(world2D, Vector2.zero);

        if (hit.collider == null)
        {
            if (_grid.TryGetGridPositionAtWorld(world2D, out Vector2Int clickedPos))
            {
                OnGridClicked(clickedPos);
                return;
            }

            Deselect();
            return;
        }

        if (hit.collider.TryGetComponent<CardBehaviour>(out var card))
        {
            if (card.Team != Team.Player)
            {
                Deselect();
                return;
            }

            ToggleSelect(card);
            return;
        }

        if (_grid.TryGetGridPositionAtWorld(world2D, out Vector2Int gridPos))
        {
            OnGridClicked(gridPos);
            return;
        }

        Deselect();
    }

    private void ToggleSelect(CardBehaviour card)
    {
        if (_selected == card)
        {
            Deselect();
            return;
        }

        _selected = card;
        _grid.ShowMoveHighlights(_grid.GetValidMoves(_selected));
    }

    private void Deselect()
    {
        _selected = null;
        _grid.ClearMoveHighlights();
    }

    private void OnGridClicked(Vector2Int gridPos)
    {
        if (_selected == null)
        {
            return;
        }

        bool moved = _grid.TryMove(_selected, gridPos);

        Deselect();

        if (moved)
        {
            _turns.EndTurn();
        }
    }
}