using UnityEngine;
using TMPro;

public class PopupTextManager : MonoBehaviour
{
    private static PopupTextManager _instance;

    [SerializeField] private Canvas _worldCanvasPrefab;

    [SerializeField] GameObject _popupTextPrefabToSpawnDamage;
    [SerializeField] GameObject _popupTextPrefabToSpawnCriticalDamage;
    
    [SerializeField] GameObject _popupTextPrefabToSpawnHeal;
    [SerializeField] GameObject _popupTextPrefabToSpawnCriticalHeal;
    
    [SerializeField] Character _testCharacter;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
            
        _instance = this;
    }

    private void Start()
    {
        
    }

    public static PopupTextManager GetInstance() { return _instance; }


    private void Update()
    {

    }
    public void BindEventOnTakeDamage(Character character)
    {
        character.OnTakeDamage += TakeDamage;
        _testCharacter = character;
    }

    public void UnBindEventOnTakeDamage(Character character)
    {
        character.OnTakeDamage -= TakeDamage;
    }
    public void BindEventOnWasHealed(Character character)
    {
        character.OnWasHealed+= WasHealed;
        _testCharacter = character;
    }

    public void UnBindEventOnWasHealed(Character character)
    {
        character.OnWasHealed -= WasHealed;
    }

    private void WasHealed(int healAmount, GameObject character)
    {
        GameObject popupText = Instantiate(_popupTextPrefabToSpawnHeal) as GameObject;
        popupText.GetComponent<PopupText>().Initialize(character, _worldCanvasPrefab);
        SetTextHealthChangedAmount(popupText, healAmount);
    }

    private void TakeDamage(int damageAmount, GameObject character)
    {
        GameObject popupText = Instantiate(_popupTextPrefabToSpawnDamage) as GameObject;
        popupText.GetComponent<PopupText>().Initialize(character, _worldCanvasPrefab);
        SetTextHealthChangedAmount(popupText, damageAmount);
    }

    private void SetTextHealthChangedAmount(GameObject popupText, int amount)
    {
        PopupText text = popupText.GetComponent<PopupText>();
        TMP_Text textMesh = text.GetTextMesh();
        textMesh.text = amount.ToString();

    }
}
