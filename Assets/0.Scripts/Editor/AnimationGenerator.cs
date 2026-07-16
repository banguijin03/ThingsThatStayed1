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

        Debug.Log($"선택된 Sprite : {sprites.Length}");

        foreach (Sprite sprite in sprites)
        {
            Debug.Log(sprite.name);
        }
    }
}