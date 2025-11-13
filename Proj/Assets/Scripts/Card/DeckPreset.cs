using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckPreset", menuName = "Item/Deck Preset", order = 1)]
public class DeckPreset : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<Card> _cards;
    public List<Card> GetCards()
    {
        return _cards;
    }
}
