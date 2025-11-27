using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Image[] ExpBar;
    [SerializeField] int ExpBarSize = 20;
    int expGain;
    private void Start()
    {
        GameData gameData = GlobalGameManager.GetInstance().GetGameData();
        expGain = 50; 
    }
    IEnumerator expBarChange()
    {
        for (int i = 0;  i < expGain; i++)
        {
            foreach(Image item in ExpBar)
            {
                yield return item; 
            }
        }
    }
}
