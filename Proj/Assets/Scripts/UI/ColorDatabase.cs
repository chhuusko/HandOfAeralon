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
    
    public Color GetCharacterColor(Character c)
    {
        if (c == null)
        {
            Debug.LogError($"{c} is null");
            return Color.white;
        }

        if (c.GetFaction() == Faction.Enemy)
        {
            return enemyColor;
        }

        return c.GetCharacterClass() switch
        {
            CharacterClass.Barbarian => barbarianColor,
            CharacterClass.Bard => bardColor,
            CharacterClass.Rogue => rogueColor,
            CharacterClass.Sorceress => sorceressColor,
            _ => Color.white
        };
    }
}
