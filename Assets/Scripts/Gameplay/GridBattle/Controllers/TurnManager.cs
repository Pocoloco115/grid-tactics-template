using System;
using UnityEngine;

public enum TurnState { Player, Enemy }

public class TurnManager : MonoBehaviour
{
    public TurnState Current { get; private set; } = TurnState.Player;

    public event Action<TurnState> TurnChanged;

    public void StartTurns(TurnState first = TurnState.Player)
    {
        Current = first;
        TurnChanged?.Invoke(Current);
    }

    public void EndTurn()
    {
        Current = (Current == TurnState.Player) ? TurnState.Enemy : TurnState.Player;
        TurnChanged?.Invoke(Current);
    }

    public bool IsPlayersTurn => Current == TurnState.Player;
}