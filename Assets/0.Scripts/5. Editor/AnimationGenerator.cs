using UnityEditor;
using UnityEngine;
using System.Linq;

public class AnimationGenerator
{
    [MenuItem("Tools/Test Sprite")]
    static void Test()
    {
        Sprite[] sprites = Selection.objects
            .OfType<Sprite>()
            .OrderBy(s => s.name)
            .ToArray();


        foreach (Sprite sprite in sprites)
        {
        }
    }
}