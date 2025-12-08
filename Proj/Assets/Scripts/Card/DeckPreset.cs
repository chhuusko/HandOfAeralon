using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckPreset", menuName = "Item/Deck Preset", order = 1)]
public class DeckPreset : ScriptableObject
{
    [SerializeField] private List<Card> _cards;
    public List<Card> GetCards()
    {
        List<Card> list = new List<Card>();

        foreach (Card card in _cards)
        {
            if (card == null) continue;

            Card clone = Instantiate(card);
            clone.hideFlags = HideFlags.DontSave;
            list.Add(clone);
        }

        return list;
    }
}
