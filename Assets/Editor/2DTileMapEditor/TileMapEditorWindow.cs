using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using System.Text;

public class TileMapEditorWindow : EditorWindow
{
    //private Tilemap targetTilemap;
    private Grid targetGrid;
    private FileType fileType = FileType.None;
    private StateGUI stateGUI = StateGUI.None;
    private string savefilePath;
    private string fileInputPath;
    private string fileName;
    private string fileExtension;

    enum StateGUI
    {
        None,
        TilemapInput,
        GridInput,
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
        fileExtension = "";
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);

        // Input
        if (GUILayout.Button("Find Files with Windows explorer"))
        {
            fileInputPath = UnityEditor.EditorUtility.OpenFilePanel("Select a File", savefilePath, "xml,json,txt,xlsx,bin"); // 탐색기
            // 경로 지정되면 파일명과 확장자 가져오기
            if (fileInputPath.Length > 0)
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
                stateGUI = StateGUI.TilemapInput;
            }
        }


        EditorGUILayout.Space(10);
        GUILayout.Label("Input", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(5));
        GUILayout.Label("Target Grid", EditorStyles.boldLabel);
        targetGrid = (Grid)EditorGUILayout.ObjectField(targetGrid, typeof(Grid), allowSceneObjects: true);
        EditorGUILayout.Space(10);


        // Output
        if (targetGrid)
        {
            if (fileName.Length < 1)
            {
                fileName = targetGrid.name;
            }

            GUILayout.Label("Output", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            fileName = EditorGUILayout.TextField("FileName : ", fileName);

            GUILayout.BeginHorizontal();
            if (stateGUI == StateGUI.None || stateGUI == StateGUI.Finish) // 작업중 다른 작업으로 이동 금지 코드
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
                GridData outputData = GridSaveLoad.MakeGridData(targetGrid);
                OuputTilemap(outputData);
                stateGUI = StateGUI.Finish;
                break;
            case StateGUI.TilemapInput:
                GridData inputData = LoadTilemap(fileInputPath);
                GridSaveLoad.MakeGrid(inputData);
                TilemapSaveLoad.ClearSpriteDictionary();
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

    GridData LoadTilemap(string _path)
    {
        string tileMapSaveExtension = System.IO.Path.GetExtension(fileInputPath); // 20230731 맴버 변수 사용 안하는 이유 -> 분리 시키고 싶어서
        GridData data = null;
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
                //data = BinaryInOutput.LoadBinary(_path);
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


    public void OuputTilemap(GridData _data)
    {
        switch (fileType)
        {
            case FileType.Binary:
                //BinaryInOutput.SaveBinary(_data, fileName, savefilePath); 바이너리 지원 종료 20230801(Tilemap에서 griddata로 변경 이후로 저장범위가 넓어 유지보수가 어려움)
                Debug.Log("Cant use Binary Output system");
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

 

 
}


[System.Serializable]
public class TileData
{
    public Vector2Int position;
    public string spriteName;
}