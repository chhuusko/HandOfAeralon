using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopNode : OverworldNode
{
    [SerializeField] private List<Card> _presetCards;

    protected override void LoadScene()
    {
        // TODO: Somehow save which presetCards to load.

        SceneManager.LoadScene(GetSceneToLoad());
    }
}
