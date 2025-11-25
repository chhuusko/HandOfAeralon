using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    private static PopupTextManager _instance;
    
    [SerializeField] GameObject PopupTextPrefabToSpawnDamage;
    [SerializeField] GameObject PopupTextPrefabToSpawnCriticalDamage;
    
    [SerializeField] GameObject PopupTextPrefabToSpawnHeal;
    [SerializeField] GameObject PopupTextPrefabToSpawnCriticalHeal;
    
    [SerializeField] Character testCharacter;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        _instance = this;
    }

    public static PopupTextManager GetInstance() { return _instance; }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testCharacter.TakeDamage(-1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            testCharacter.TakeDamage(1);
        }
    }
    public void BindEventOnHealthChanged(Character character)
    {
        character.OnHealthChanged += HealthChanged;
        testCharacter = character;
    }

    public void UnBindEventOnHealthChanged(Character character)
    {
        character.OnHealthChanged -= HealthChanged;
    }

    private void HealthChanged(int newHealth, int amount)
    {
        GameObject popupText = null;
        
        if(amount > 0)
        {
            popupText = Instantiate(PopupTextPrefabToSpawnHeal) as GameObject;
        }
        else if(amount < 0)
        {
            popupText = Instantiate(PopupTextPrefabToSpawnDamage) as GameObject;
        }

        DamagePopupText text = popupText.GetComponent<DamagePopupText>();
        TextMesh textMesh = text.GetComponent<TextMesh>();

        if (amount < 0)
            amount *= -1;

        textMesh.text = amount.ToString();
    }

}
