using UnityEngine;
[CreateAssetMenu(fileName = "Echoing Aid", menuName = "Item/Card Data/Echoing Aid", order = 1)]
public class EchoingAid : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ApplyCardEffectOnFactionMember effect;
    public override void PlayCard()
    {
        CardHandManager.GetInstance().turnEffects.Add(effect);
    }
}
