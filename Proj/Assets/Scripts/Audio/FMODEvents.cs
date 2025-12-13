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
    [SerializeField] private string damageParameter;
    
    public EventReference BarbarianTakeDamage => barbarianTakeDamage;
    public EventReference BarbarianDeath => barbarianDeath;
    public EventReference RogueTakeDamage => rogueTakeDamage;
    public EventReference RogueDeath => rogueDeath;
    public EventReference BardTakeDamage => bardTakeDamage;
    public EventReference BardDeath => bardDeath;
    public EventReference SorceressTakeDamage => sorceressTakeDamage;
    public EventReference SorceressDeath => sorceressDeath;
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
