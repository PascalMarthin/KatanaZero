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
    private string savefilePath ;
    private string fileInputPath ;
    private string fileOutputPath ;
    private string fileName;

    enum FileType
    {
        None,
        Finish,

        // Output
        Binary,
        Txt,
        Json,
        XML,

        // Input
        Findfile,
        Input,
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
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);

        // Input
        if (GUILayout.Button("Find Files with Windows explorer"))
        {
            fileInputPath = UnityEditor.EditorUtility.OpenFilePanel("Select a File", savefilePath, "xml,json,text"); // Ž����
        }
        EditorGUILayout.Space(10);
        if (fileInputPath != "")
        {
            GUILayout.Label(fileInputPath);
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Input") && fileType == FileType.None)
            {
                fileType = FileType.Input;
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
            if (fileType == FileType.None || fileType == FileType.Finish) // �۾��� �ٸ� �۾����� �̵� ���� �ڵ�
            {
                if (GUILayout.Button("Binary"))
                {
                    fileType = FileType.Binary;
                }
                if (GUILayout.Button("Txt"))
                {
                    fileType = FileType.Txt;
                }
                if (GUILayout.Button("Json"))
                {
                    fileType = FileType.Json;
                }
                if (GUILayout.Button("XML"))
                {
                    fileType = FileType.XML;
                }
            }

            GUILayout.EndHorizontal();
        }


    }

    void OnInspectorUpdate()
    {
        switch (fileType)
        { 
            case FileType.Binary:
                break; 
            case FileType.Txt:
                break;
            case FileType.Json:
                break;
            case FileType.XML:
                List<TileData> data = SaveTilemap();
                SaveXML(data, savefilePath + "/" + fileName + ".xml");
                fileType = FileType.Finish;
                break;
            case FileType.Input:
                LoadTilemap(LoadXML(fileInputPath));
                fileType = FileType.Finish;
                break;

            case FileType.Finish:
                fileName = "";
                fileType = FileType.None;
                break;
            case FileType.None:
            default:
                break;
        }
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

    void LoadTilemap(List<TileData> _data)
    {
        if(fileName == "")
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
                int index = tileSpritesStr.LastIndexOf('_');
                string folderStr = "Sprites/Map/Bunker/";
                Sprite[] sprites;
                if (index != -1)
                {
                    folderStr += tileSpritesStr.Substring(0, index);
                    sprites = Resources.LoadAll<Sprite>(folderStr);
                    if(sprites.Length > 0)
                    {
                        foreach(Sprite spr in sprites)
                        {
                            targetSprites.Add(spr.name, spr);
                        }
                    }
                    else
                    {
                        Debug.Log("We cant find sprites Folder -> " + tileSpritesStr);
                        continue;
                    }

                    if(!targetSprites.TryGetValue(tileSpritesStr, out sprite))
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

    void SaveXML(List<TileData> _data, string _filePath)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TileData>));
        StreamWriter writer = new StreamWriter(_filePath);
        xmlSerializer.Serialize(writer, _data);
        writer.Close();
    }


    List<TileData> LoadXML(string _filePath)
    {
        var serializer = new XmlSerializer(typeof(List<TileData>));
        using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
        {
            fileName = System.IO.Path.GetFileNameWithoutExtension(_filePath);
            List<TileData> loadData = (List<TileData>)serializer.Deserialize(stream);
            return loadData;
        }
    }
}


[System.Serializable]
public class TileData
{
    public Vector2Int position;
    public string spriteName;
}