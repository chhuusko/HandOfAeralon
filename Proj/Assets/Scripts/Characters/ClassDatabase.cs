using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "Character/ClassDatabase")]
public class ClassDatabase : ScriptableObject
{
    public List<ClassData> Classes;
}
