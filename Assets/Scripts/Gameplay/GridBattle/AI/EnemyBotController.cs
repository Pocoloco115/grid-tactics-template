using System.Collections.Generic;
using UnityEngine;

public class EnemyBotController : MonoBehaviour
{
    [SerializeField] private GridManager _grid;
    [SerializeField] private TurnManager _turns;
    [SerializeField] private BattleManager _battle;

    private void OnEnable()
    {
        _turns.TurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        _turns.TurnChanged -= OnTurnChanged;
    }

    private void OnTurnChanged(TurnState state)
    {
        if (state != TurnState.Enemy) return;

        DoEnemyMove();
        _turns.EndTurn();
    }

    private void DoEnemyMove()
    {
        var candidates = new List<CardBehaviour>();
        foreach (var c in _battle.EnemyCards)
        {
            if (c == null) continue;
            if (_grid.GetValidMoves(c).Count > 0)
                candidates.Add(c);
        }

        if (candidates.Count == 0) return;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        var moves = _grid.GetValidMoves(chosen);
        var dest = moves[Random.Range(0, moves.Count)];

        _grid.TryMove(chosen, dest);
    }
}