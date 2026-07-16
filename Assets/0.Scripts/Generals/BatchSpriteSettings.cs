/*using UnityEditor;
using UnityEngine;

public class BatchSpriteSettings
{
    [MenuItem("Tools/Set Character Sprites")]
    public static void SetCharacterSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.Contains("Character"))
                continue;

            TextureImporter importer =
                AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
                continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            importer.filterMode = FilterMode.Point;
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;

            importer.spritePixelsPerUnit = 32;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            count++;
        }

        Debug.Log($"¿Ï·á : {count}°³");
    }
}*/