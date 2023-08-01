using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonInOutput
{
    static public void SaveJson(GridData _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".json";

        string json = JsonUtility.ToJson(_data);
        File.WriteAllText(saveFilePath, json); // 쓰고 바로 닫기
    }

    static public string TransJson(GridData _data)
    {
        return JsonUtility.ToJson(_data);
    }


    static public GridData LoadJson(string _filePath)
    {
        string jsonContent;
        using (StreamReader reader = new StreamReader(_filePath))
        {
            jsonContent = reader.ReadToEnd();
        }

        GridData tileData = JsonUtility.FromJson<GridData>(jsonContent);
        return tileData;
    }
}
