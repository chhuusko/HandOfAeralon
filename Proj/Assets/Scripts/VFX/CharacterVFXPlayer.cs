using System;
using UnityEngine;

public class CharacterVFXPlayer : MonoBehaviour
{
    [SerializeField] private VFXPlayer characterVFX;
    private VFXPlayer _activeVFX;

    public void ShowPreview()
    {
        if (_activeVFX != null) return;

        _activeVFX = Instantiate(characterVFX, transform);
        _activeVFX.Play(transform.position, transform.forward);
    }

    public void HidePreview()
    {
        if (_activeVFX == null) return;

        Destroy(_activeVFX.gameObject);
        _activeVFX = null;
    }
}

