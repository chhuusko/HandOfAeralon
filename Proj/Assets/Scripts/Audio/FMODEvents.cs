using System;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance { get; private set; }

    [Header("Player SFX")] 
    [SerializeField] private EventReference barbarianTakeDamage;
    [SerializeField] private EventReference barbarianDeath;
    [SerializeField] private EventReference rogueTakeDamage;
    [SerializeField] private EventReference rogueDeath;
    [SerializeField] private EventReference bardTakeDamage;
    [SerializeField] private EventReference bardDeath;
    [SerializeField] private EventReference sorceressTakeDamage;
    [SerializeField] private EventReference sorceressDeath;
    [SerializeField] private EventReference barbarianFootsteps, bardFootsteps, rogueFootsteps, sorceressFootsteps;;
    [SerializeField] private string damageParameter;


    [Header("Tile SFX")]
    [SerializeField] private EventReference enterPoisonTile;
    [SerializeField] private EventReference enterLavaTile;

    [Header("UI SFX")]
    [SerializeField] private EventReference menuOpened;
    [SerializeField] private EventReference menuClosed;
    [SerializeField] private EventReference buttonClick;

    [Header("Game Event SFX")]
    [SerializeField] private EventReference playerDeafeted;
    [SerializeField] private EventReference playerVictory;


    public EventReference BarbarianTakeDamage => barbarianTakeDamage;
    public EventReference BarbarianDeath => barbarianDeath;
    public EventReference RogueTakeDamage => rogueTakeDamage;
    public EventReference RogueDeath => rogueDeath;
    public EventReference BardTakeDamage => bardTakeDamage;
    public EventReference BardDeath => bardDeath;
    public EventReference SorceressTakeDamage => sorceressTakeDamage;
    public EventReference SorceressDeath => sorceressDeath;
    public EventReference BarbarianFootsteps => barbarianFootsteps;
    public EventReference BardFootsteps => bardFootsteps;
    public EventReference RogueFootsteps => rogueFootsteps;
    public EventReference SorceressFootsteps => sorceressFootsteps;
    public EventReference EnterPoisonTile => enterPoisonTile;
    public EventReference EnterLavaTile => enterLavaTile;
    public EventReference MenuOpened => menuOpened;
    public EventReference MenuClosed => menuClosed;
    public EventReference ButtonClick => buttonClick;
    public EventReference PlayerDefeated => playerDeafeted;
    public EventReference PlayerVictory => playerVictory;
    public string DamageParameter => damageParameter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
