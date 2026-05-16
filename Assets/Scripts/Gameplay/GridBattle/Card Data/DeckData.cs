using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckData", menuName = "Scriptable Objects/DeckData")]
public class DeckData : ScriptableObject
{
    [SerializeField] private List<CardData> _cards = new();
    public IReadOnlyList<CardData> Cards => _cards;
    public int MaxCards => 3;
    public int CountClampled => Mathf.Min(_cards.Count, MaxCards);
}
