using UnityEngine;

[CreateAssetMenu(fileName = "TraitData", menuName = "Traits/TraitData")]
public class TraitData : StatusEffectData
{
    public bool IsPositive;
    public CharacterClass Class;
}
