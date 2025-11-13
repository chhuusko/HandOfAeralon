using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static LevelManager _instance;
    private string[] LevelFileList; 

    public static LevelManager GetInstance() {  return _instance; }
    private void Awake()
    {
        _instance = this;
        LevelFileList = Directory.GetFiles("Assets/JSON BattleGrids");
    }


}
