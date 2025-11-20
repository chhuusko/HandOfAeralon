
using UnityEngine;

[CreateAssetMenu(fileName = "InfoPanel", menuName = "UI/InfoPanel", order = 1)]
public class InfoPanel : ScriptableObject
{
    [SerializeField] public string title, description;

}