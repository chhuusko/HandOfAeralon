
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "Item/Card Data", order = 1)]
public class Card : ScriptableObject
{
    [Header("Info")]
    public string title;
    public string description;
    public Sprite icon;
    
    public void OnPlay()
    {
        //när den spelas
    }
}

