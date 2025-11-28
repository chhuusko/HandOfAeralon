using UnityEngine;

public class CutSceneManager : MonoBehaviour
{

    private static CutSceneManager _instance;
    
    [SerializeField] private Animator _animatorFadeTexture;
    [SerializeField] private Animator _animatorSmoke;


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
        if(_animatorFadeTexture != null)
            _animatorFadeTexture.enabled = false;

        if (_animatorSmoke != null)
            _animatorSmoke.enabled = false;
    }

    private void PlayFadeIn()
    {
        if (_animatorFadeTexture != null)
        {
            _animatorFadeTexture.enabled = true;
            _animatorFadeTexture.Play("CutsceneFadeIn");
        }


        if (_animatorSmoke!= null)
        {
            _animatorSmoke.enabled = true;
            _animatorSmoke.Play("FadeOutSmoke");
        }
    }

}
