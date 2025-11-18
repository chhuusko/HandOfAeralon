using UnityEngine;

public class PortraitButton : MonoBehaviour
{
    private Character _character;

    public void SetCharacter(Character character)
    {
        _character = character;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        CombatUI.Instance.LoadAbilities(_character);
    }
}
