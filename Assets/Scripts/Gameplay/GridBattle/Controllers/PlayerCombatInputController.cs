using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCombatInputController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GridManager _grid;
    [SerializeField] private TurnManager _turns;
    [SerializeField] private UIActionPanel _actionPanel;

    private CardBehaviour _selected;
    private UIActionType _selectedAction = UIActionType.None;

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
        if (_actionPanel != null)
        {
            _actionPanel.SelectionChanged += OnActionSelected;
        }

        _click.Enable();
    }

    private void OnDisable()
    {
        if (_actionPanel != null)
        {
            _actionPanel.SelectionChanged -= OnActionSelected;
        }

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

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
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
        _selectedAction = UIActionType.None;

        if (_actionPanel != null)
        {
            _actionPanel.Show();
        }

        _grid.ClearMoveHighlights();
    }

    private void Deselect()
    {
        _selected = null;
        _selectedAction = UIActionType.None;

        if (_actionPanel != null)
        {
            _actionPanel.Hide();
        }

        _grid.ClearMoveHighlights();
    }

    private void OnActionSelected(UIActionItem item)
    {
        if (_selected == null || item == null)
        {
            _selectedAction = UIActionType.None;
            _grid.ClearMoveHighlights();
            return;
        }

        _selectedAction = item.Action;

        if (_selectedAction == UIActionType.Attack)
        {
            _grid.ShowMoveHighlights(_grid.GetValidAttackOffsets(_selected));
            return;
        }

        _grid.ShowMoveHighlights(_grid.GetValidMoves(_selected));
    }

    private void OnGridClicked(Vector2Int gridPos)
    {
        if (_selected == null)
        {
            return;
        }

        if (_selectedAction != UIActionType.Move)
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