using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public Ability Ability { get; set; }
    [SerializeField] private Button _button;
    public Button Button => _button;
    
    public void OnClick()
    {
        Selector._instance.PreviewAbilityRange(Ability);
    }
}
