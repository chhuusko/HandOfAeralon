using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool isDeveloperMode;
    private InputController _controller;

    void Awake()
    {
        _controller = new InputController();
    }

    private void OnEnable()
    {
        _controller.Enable();
        _controller.Developer.SkipLevel.performed += SkipLevel;
        _controller.Player.EndTurn.performed += EndTurn;
    }

    private void OnDisable()
    {
        _controller.Enable();
        _controller.Developer.SkipLevel.performed -= SkipLevel;
    }
    
    private void SkipLevel(InputAction.CallbackContext context)
    {
        if (isDeveloperMode)
            LevelManager.GetInstance().StartNextLevel();
    }
    private void EndTurn(InputAction.CallbackContext context)
    {
        CombatUI.Instance.EndTurn();
    }
    

}
