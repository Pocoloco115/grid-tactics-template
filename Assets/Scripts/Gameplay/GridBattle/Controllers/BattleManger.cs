using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private bool _autoStart = true;

    [Header("Refs")]
    [SerializeField] private GridManager _grid;
    [SerializeField] private TurnManager _turns;

    [Header("Decks")]
    [SerializeField] private DeckData _playerDeck;
    [SerializeField] private DeckData _enemyDeck;

    [Header("Spawns (max 3)")]
    [SerializeField] private Vector2Int[] _playerSpawns = { new(0,0), new(1,0), new(2,0) };
    [SerializeField] private Vector2Int[] _enemySpawns  = { new(0,4), new(1,4), new(2,4) };

    [SerializeField] private int _playerSpawnY = 0;
    [SerializeField] private int _enemySpawnY = 4;

    public readonly List<CardBehaviour> PlayerCards = new();
    public readonly List<CardBehaviour> EnemyCards = new();

    private void Start()
    {
        if (_autoStart) StartBattle();
    }

    public void StartBattle()
    {
        PlayerCards.Clear();
        EnemyCards.Clear();

        SpawnDeck(_playerDeck, Team.Player, _playerSpawnY, PlayerCards);
        SpawnDeck(_enemyDeck, Team.Enemy, _enemySpawnY, EnemyCards);

        _turns.StartTurns(TurnState.Player);
    }

    private void SpawnDeck(DeckData deck, Team team, int spawnY, List<CardBehaviour> output)
    {
        if (deck == null) return;

        const int gapCells = 1;
        int maxBySpacing = (_grid.Width + gapCells) / (1 + gapCells);
        int count = Mathf.Min(deck.CountClampled, maxBySpacing);
        if (count <= 0) return;

        int clampedSpawnY = Mathf.Clamp(spawnY, 0, _grid.Height - 1);
        int totalWidthNeeded = count + (count - 1) * gapCells;
        int startX = totalWidthNeeded <= _grid.Width
            ? (_grid.Width - totalWidthNeeded) / 2
            : (_grid.Width - count) / 2;

        for (int i = 0; i < count; i++)
        {
            var data = deck.Cards[i];
            if (data == null) continue;

            int x = totalWidthNeeded <= _grid.Width ? startX + i * (1 + gapCells) : startX + i;
            var pos = new Vector2Int(x, clampedSpawnY);

            var card = _grid.SpawnCard(data, pos, team);
            if (card != null) output.Add(card);
        }
    }
}