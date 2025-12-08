using UnityEngine;
[CreateAssetMenu(fileName = "Echoing Harm", menuName = "Item/Card Data/Echoing Harm", order = 1)]
public class EchoingHarm : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private ApplyCardEffectOnFactionMember effect;
    public override void PlayCard()
    {
        CardHandManager.GetInstance().turnEffects.Add(Instantiate(effect));
    }
}
