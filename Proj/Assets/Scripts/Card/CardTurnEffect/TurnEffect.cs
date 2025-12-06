using System.Collections;
using UnityEngine;

public class TurnEffect : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        CardHandManager.onCardTargetCharacter += Effect;
    }
    private void OnDisable()
    {
        CardHandManager.onCardTargetCharacter -= Effect;
    }
    protected virtual void Effect(Character character, Card card)
    {

    }
}
