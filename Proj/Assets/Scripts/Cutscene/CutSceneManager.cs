using UnityEngine;

public class CutSceneManager : MonoBehaviour
{

    private static CutSceneManager _instance;
    
    [SerializeField] private Animator _animator;


    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
            
        }

        _instance = this;
    }

    private void OnEnable()
    {
        CombatEventManager.OnEnterCombatStateIntroCinematic += PlayFadeIn;
    }

    private void OnDisable()
    {
        
    }

    void Start()
    {
        if(_animator != null)
            _animator.enabled = false;
    }

    private void PlayFadeIn()
    {
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.Play("CutsceneFadeIn");
        }
    }

}
