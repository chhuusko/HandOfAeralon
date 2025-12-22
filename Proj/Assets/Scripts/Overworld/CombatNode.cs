using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatNode : OverworldNode
{
    [SerializeField] private int coinRewardAmount = 300;
    protected override void LoadScene()
    {
        // TODO: Somehow save how much coins should be rewarded for winning.

        SceneManager.LoadScene(GetSceneToLoad());
    }
}
