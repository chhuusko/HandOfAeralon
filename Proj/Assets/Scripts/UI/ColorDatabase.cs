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
    
    public Color BarbarianColor;
    public Color BardColor;
    public Color RogueColor;
    public Color SorceressColor;
    public Color EnemyColor;
    
    public Color GetCharacterColor(Character c)
    {
        if (c == null)
        {
            Debug.LogError($"{c} is null");
            return Color.white;
        }

        if (c.GetFaction() == Faction.Enemy)
        {
            return EnemyColor;
        }

        return c.GetCharacterClass() switch
        {
            CharacterClass.Barbarian => BarbarianColor,
            CharacterClass.Bard => BardColor,
            CharacterClass.Rogue => RogueColor,
            CharacterClass.Sorceress => SorceressColor,
            _ => Color.white
        };
    }
}
