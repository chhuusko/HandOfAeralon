using UnityEngine;

[CreateAssetMenu(fileName = "Color Database", menuName = "UI/Color Database")]
public class ColorDatabase : ScriptableObject
{
    private static ColorDatabase instance;

    public static ColorDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ColorDatabase>("Color Database");
            }
            return instance;
        }
    }
    
    public Color barbarianColor;
    public Color bardColor;
    public Color rogueColor;
    public Color sorceressColor;
    public Color enemyColor;
}
