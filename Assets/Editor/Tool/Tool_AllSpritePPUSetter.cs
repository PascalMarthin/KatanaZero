using UnityEditor;
using UnityEngine;
using System.IO;

public class Tool_AllSpritePPUSetter : EditorWindow
{
    private int pixelsPerUnit = 100;
    private bool isStart = false;
    private bool isEnd = false;

    // 처리 양
    private string currentPath = "";
    private int currentSpriteIndex = 0;
    private int goalSpriteppu = 0;

    [MenuItem("Tools/Set PPU For All Sprites")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Tool_AllSpritePPUSetter));
    }

    void OnGUI()
    {
        GUILayout.Label("Target PPU", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        pixelsPerUnit = EditorGUILayout.IntSlider("PixelsPerUnit", pixelsPerUnit, 0, 200);
        GUILayout.Space(10);

        if (GUILayout.Button("Go"))
        {
            isStart = true;
        }

        if(isStart)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("CurrentPath : " + currentPath);
        }

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        if (isEnd)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Done!");
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Change " + currentSpriteIndex  + " sprites to " + goalSpriteppu + " PixelsPerUnit");
        }
    }

    void OnInspectorUpdate()
    {
        if(isStart)
        {
            isEnd = false;

            goalSpriteppu = pixelsPerUnit;
            SetPPUForAllSprites(pixelsPerUnit);

            isStart = false;
            isEnd = true;
        }

    }

    private void SetPPUForAllSprites(int _ppu)
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite");

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            currentPath = assetPath;
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                if(importer.spritePixelsPerUnit != _ppu)
                {
                    importer.spritePixelsPerUnit = _ppu;
                    ++currentSpriteIndex;
                }

                importer.filterMode = FilterMode.Point; // 필터 Off
                importer.textureCompression = TextureImporterCompression.Uncompressed; // 압축 Off
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // 적용
            }
        }
    }
}
