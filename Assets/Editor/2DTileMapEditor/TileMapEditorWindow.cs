using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

public class TileMapEditorWindow : EditorWindow
{
    private Tilemap tilemap;
    private FileType fileType = FileType.None;
    private StateGUI stateGUI = StateGUI.None;
    private string savefilePath ;
    private string fileInputPath ;
    private string fileOutputPath ;
    private string fileName;
    private string fileExtension;
    
    enum StateGUI
    {
        None,
        Input,
        Output,
        Finish,
    }

    enum FileType
    {
        None,
        Binary,
        Txt,
        Json,
        XML,
        Xlsx,
    }

    [MenuItem("Tools/TestSerialize")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TileMapEditorWindow));
    }

    void OnEnable()
    {
        savefilePath = Application.persistentDataPath;
        fileName = "";
        fileInputPath = "";
        fileOutputPath = "";
        fileExtension = "";
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);

        // Input
        if (GUILayout.Button("Find Files with Windows explorer"))
        {
            fileInputPath = UnityEditor.EditorUtility.OpenFilePanel("Select a File", savefilePath, "xml,json,txt,xlsx,bin"); // Ž����
            // ��� �����Ǹ� ���ϸ�� Ȯ���� ��������
            if(fileInputPath.Length > 0)
            {
                fileName = System.IO.Path.GetFileNameWithoutExtension(fileInputPath);
                fileExtension = System.IO.Path.GetExtension(fileInputPath);
            }
        }
        EditorGUILayout.Space(10);
        if (fileInputPath != "")
        {
            GUILayout.Label(fileInputPath);
            EditorGUILayout.Space(10);
            GUILayout.Label("FileName : " + fileName);
            GUILayout.Label("FileExtension : " + fileExtension);
            
            if (GUILayout.Button("Input") && fileType == FileType.None)
            {
                stateGUI = StateGUI.Input;
            }
        }


        EditorGUILayout.Space(10);
        GUILayout.Label("Input", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
        GUILayout.Label("Target TileMap", EditorStyles.boldLabel);
        tilemap = (Tilemap)EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), allowSceneObjects: true);
        EditorGUILayout.Space(10);


        // Output
        if (tilemap)
        {
            if(fileName.Length < 1)
            {
                fileName = tilemap.name;
            }

            GUILayout.Label("Output", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            fileName = EditorGUILayout.TextField("FileName : ", fileName);

            GUILayout.BeginHorizontal();
            if (stateGUI == StateGUI.None || stateGUI == StateGUI.Finish) // �۾��� �ٸ� �۾����� �̵� ���� �ڵ�
            {
                if (GUILayout.Button("Binary"))
                {
                    fileType = FileType.Binary;
                    stateGUI = StateGUI.Output;
                }
                if (GUILayout.Button("Txt"))
                {
                    fileType = FileType.Txt;
                    stateGUI = StateGUI.Output;
                }
                if (GUILayout.Button("Json"))
                {
                    fileType = FileType.Json;
                    stateGUI = StateGUI.Output;
                }
                if (GUILayout.Button("XML"))
                {
                    fileType = FileType.XML;
                    stateGUI = StateGUI.Output;
                }
            }

            GUILayout.EndHorizontal();
        }


    }

    void OnInspectorUpdate()
    {
        switch (stateGUI)
        {
            case StateGUI.Output:
                List<TileData> outputData = SaveTilemap();
                OuputTilemap(outputData);
                stateGUI = StateGUI.Finish;
                break;
            case StateGUI.Input:
                List<TileData> inputData = LoadTilemap(fileInputPath);
                MakeTilemap(inputData);
                stateGUI = StateGUI.Finish;
                break;

            case StateGUI.Finish:
                fileName = "";
                fileType = FileType.None;
                stateGUI = StateGUI.None;
                break;
            case StateGUI.None:
            default:
                break;
        }
    }

    List<TileData> LoadTilemap(string _path)
    {
        string tileMapSaveExtension = System.IO.Path.GetExtension(fileInputPath); // 20230731 �ɹ� ���� ��� ���ϴ� ���� -> �и� ��Ű�� �;
        List<TileData> data = null;
        if (tileMapSaveExtension == ".xml")
        {
            fileType = FileType.XML;
        }
        else if (tileMapSaveExtension == ".bin")
        {
            fileType = FileType.Binary;
        }
        else if (tileMapSaveExtension == ".xlsx")
        {
            fileType = FileType.Xlsx;
        }
        else if (tileMapSaveExtension == ".json")
        {
            fileType = FileType.Json;
        }
        else if (tileMapSaveExtension == ".txt")
        {
            fileType = FileType.Txt;
        }

        switch (fileType)
        {
            case FileType.Binary:
                data = BinaryInOutput.LoadBinary(_path);
                break;
            case FileType.Txt:
                data = TXTInOutput.LoadTxt(_path);
                break;
            case FileType.Json:
                data = JsonInOutput.LoadJson(_path);
                break;
            case FileType.XML:
                data = XMLInOutput.LoadXML(_path);
                break;
            case FileType.Xlsx:
                break;
            default:
                Debug.Log("Dont know Extension -> " + tileMapSaveExtension);
                break;
        }
        fileType = FileType.None;
        return data;
    }

    public List<TileData> SaveTilemap()
    {
        var bounds = tilemap.cellBounds;
        var allTiles = tilemap.GetTilesBlock(bounds);

        List<TileData> tileDataList = new List<TileData>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int z = 0; z < bounds.size.z; z++)
                {
                    var localPlace = new Vector3Int(x, y, z) + bounds.min; // �ּҰ����� ���� �̵�
                    TileBase tileBase = allTiles[x + y * bounds.size.x];
                    Tile tile = tileBase as Tile;
                    if (tile != null) // Ÿ���� �ִ� ��쿡�� ����Ʈ�� �߰�
                    {
                        var tileData = new TileData { position = (Vector2Int)localPlace, spriteName = tile.sprite.name };
                        tileDataList.Add(tileData);
                    }
                }
            }
        }
        return tileDataList;
    }

    public void OuputTilemap(List<TileData> _data)
    {
        switch (fileType)
        {
            case FileType.Binary:
                BinaryInOutput.SaveBinary(_data, fileName, savefilePath);
                break;
            case FileType.Txt:
                TXTInOutput.SaveTxt(_data, fileName, savefilePath);
                break;
            case FileType.Json:
                JsonInOutput.SaveJson(_data, fileName, savefilePath);
                break;
            case FileType.XML:
                XMLInOutput.SaveXML(_data, fileName, savefilePath);
                break;
            case FileType.Xlsx:
                break;
            default:
                Debug.Log("fileType is null!");
                break;
        }
        fileType = FileType.None;
    }

    void MakeTilemap(List<TileData> _data)
    {
        if (fileName == "")
        {
            fileName = "Default";
        }

        // Grid ������Ʈ�� �ִ� �θ� ���� ������Ʈ�� ã�ų�, ���ٸ� ���� ����
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
        }

        GameObject tilemapGameObject = new GameObject(fileName);

        // Tilemap ������Ʈ�� Tilemap Renderer ������Ʈ�� ���� ������Ʈ�� �߰�
        Tilemap inputTileMap = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();

        tilemapGameObject.transform.SetParent(grid.transform);


        Dictionary<string, Sprite> targetSprites = new Dictionary<string, Sprite>(); // �ε� ��������Ʈ

        // �����͸� TileMap���ٰ� ����
        foreach (TileData tileData in _data)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            string tileSpritesStr = tileData.spriteName;

            // ��������Ʈ �迭���� �̸����� ��������Ʈ�� ã��
            Sprite sprite;
            if (!targetSprites.TryGetValue(tileSpritesStr, out sprite))
            {
                // ������ Load
                string folderStr = "Sprites/Map/Bunker/";
                int index = tileSpritesStr.LastIndexOf('_');
                Sprite[] sprites;
                if (index != -1)
                {
                    folderStr += tileSpritesStr.Substring(0, index);
                    sprites = Resources.LoadAll<Sprite>(folderStr);
                    if (sprites.Length > 0)
                    {
                        foreach (Sprite spr in sprites)
                        {
                            targetSprites.Add(spr.name, spr);
                        }
                    }
                    else
                    {
                        Debug.Log("We cant find sprites Folder -> " + tileSpritesStr);
                        continue;
                    }

                    if (!targetSprites.TryGetValue(tileSpritesStr, out sprite))
                    {
                        Debug.Log("We cant find sprite -> " + tileSpritesStr);
                        continue;
                    }
                }
                else
                {
                    Debug.Log("This sprite is not allow -> " + tileSpritesStr);
                    continue;
                }
            }
            // Ÿ�� ���� �� ���ҽ� ��������
            tile.sprite = sprite;

            // Ÿ���� Ÿ�ϸʿ� ������
            inputTileMap.SetTile(new Vector3Int(tileData.position.x, tileData.position.y, 0), tile);
        }
    }
}


[System.Serializable]
public class TileData
{
    public Vector2Int position;
    public string spriteName;
}