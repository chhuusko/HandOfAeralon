using UnityEngine;

[CreateAssetMenu(fileName = "Reapers Ledger", menuName = "Item/Card Data/Reapers Ledger", order = 1)]
public class ReapersLedger : Card
{
    [SerializeField] private int baseDamage;
    [SerializeField] private int damageIncrease;

    public override void PlayCardOnTarget(Character character)
    {
        if (character != null)
        {
            character.TakeDamage(GetDamage(character));
            if (character.GetCurrentHealth() <= 0)
            {
                GlobalGameManager.GetInstance().ReapersLedgerKillChange(1);
            }
        }
    }
    public override int GetDamage(Character character)
    {
        int damage = baseDamage + (GlobalGameManager.GetInstance().GetGameData().reapersLedgerKills * damageIncrease);
        return Mathf.RoundToInt(character.GetStatusEffectManager().ModifyIncomingDamage(damage, null));

    }
    public override string GetDescription()
    {
        int damage = baseDamage + (GlobalGameManager.GetInstance().GetGameData().reapersLedgerKills * damageIncrease);
        return description.Replace("{damage}", ""+damage);
    }
}
