using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BoardType
    {
        Hex,
        Rectangle
    }

    [SerializeField] private bool _autoStart = true;
    [SerializeField] private BoardType _boardType = BoardType.Hex;

    [Header("Refs")]
    [SerializeField] private HexGridManager _hexGrid;
    [SerializeField] private RectGridManager _rectGrid;
    [SerializeField] private PlayerCombatInputController _playerInput;
    [SerializeField] private EnemyBotController _enemyBot;
    [SerializeField] private TurnManager _turns;

    [Header("Decks")]
    [SerializeField] private DeckData _playerDeck;
    [SerializeField] private DeckData _enemyDeck;

    public readonly List<CardBehaviour> PlayerCards = new();
    public readonly List<CardBehaviour> EnemyCards = new();

    private GridManager _grid;

    private void Awake()
    {
        ResolveGrid();
    }

    private void Start()
    {
        if (_autoStart)
        {
            StartBattle();
        }
    }

    private void ResolveGrid()
    {
        if (_boardType == BoardType.Hex)
        {
            _grid = _hexGrid;
            if (_hexGrid != null)
            {
                _hexGrid.gameObject.SetActive(true);
            }

            if (_rectGrid != null)
            {
                _rectGrid.gameObject.SetActive(false);
            }
        }
        else
        {
            _grid = _rectGrid;
            if (_rectGrid != null)
            {
                _rectGrid.gameObject.SetActive(true);
            }

            if (_hexGrid != null)
            {
                _hexGrid.gameObject.SetActive(false);
            }
        }

        if (_playerInput != null)
        {
            _playerInput.SetGrid(_grid);
        }

        if (_enemyBot != null)
        {
            _enemyBot.SetGrid(_grid);
        }
    }

    public void StartBattle()
    {
        if (_grid == null)
        {
            ResolveGrid();
        }

        PlayerCards.Clear();
        EnemyCards.Clear();

        SpawnDeck(_playerDeck, Team.Player, PlayerCards);
        SpawnDeck(_enemyDeck, Team.Enemy, EnemyCards);

        _turns.StartTurns(TurnState.Player);
    }

    private void SpawnDeck(DeckData deck, Team team, List<CardBehaviour> output)
    {
        if (deck == null)
        {
            return;
        }

        if (_grid == null)
        {
            return;
        }

        int row = team == Team.Player ? 0 : _grid.Height - 1;
        List<Vector2Int> spawnSlots = BattleSpawnUtility.GetSpawnSlots(_grid, row, deck.CountClampled);
        int count = spawnSlots.Count;
        if (count <= 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var data = deck.Cards[i];
            if (data == null)
            {
                continue;
            }

            Vector2Int pos = spawnSlots[i];

            var card = _grid.SpawnCard(data, pos, team);
            if (card != null)
            {
                output.Add(card);
            }
        }
    }

}