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
            fileInputPath = UnityEditor.EditorUtility.OpenFilePanel("Select a File", savefilePath, "xml,json,text"); // 탐색기
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
            if (fileType == FileType.None || fileType == FileType.Finish) // 작업중 다른 작업으로 이동 금지 코드
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
                    var localPlace = new Vector3Int(x, y, z) + bounds.min; // 최소값으로 기준 이동
                    TileBase tileBase = allTiles[x + y * bounds.size.x];
                    Tile tile = tileBase as Tile;
                    if (tile != null) // 타일이 있는 경우에만 리스트에 추가
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
        
        // Grid 컴포넌트가 있는 부모 게임 오브젝트를 찾거나, 없다면 새로 생성
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
        }

        GameObject tilemapGameObject = new GameObject(fileName);

        // Tilemap 컴포넌트와 Tilemap Renderer 컴포넌트를 게임 오브젝트에 추가
        Tilemap inputTileMap = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();

        tilemapGameObject.transform.SetParent(grid.transform);


        Dictionary<string, Sprite> targetSprites = new Dictionary<string, Sprite>(); // 로드 스프라이트

        // 데이터를 TileMap에다가 적용
        foreach (TileData tileData in _data)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            string tileSpritesStr = tileData.spriteName;

            // 스프라이트 배열에서 이름으로 스프라이트를 찾음
            Sprite sprite;
            if (!targetSprites.TryGetValue(tileSpritesStr, out sprite))
            {
                // 없으면 Load
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
            // 타겟 폴더 내 리소스 가져오기
            tile.sprite = sprite;

            // 타일을 타일맵에 적용함
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