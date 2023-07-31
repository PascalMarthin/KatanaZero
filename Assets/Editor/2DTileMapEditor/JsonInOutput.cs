using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonInOutput
{
    static public void SaveJson(List<TileData> _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".json";

        TileDataListWrapper<TileData> wrapper = new TileDataListWrapper<TileData>();
        wrapper.data = _data;
        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(saveFilePath, json); // 쓰고 바로 닫기
    }

    static public string TransJson(List<TileData> _data)
    {
        TileDataListWrapper<TileData> wrapper = new TileDataListWrapper<TileData>();
        wrapper.data = _data;
        return JsonUtility.ToJson(wrapper);
    }


    static public List<TileData> LoadJson(string _filePath)
    {
        string jsonContent;
        using (StreamReader reader = new StreamReader(_filePath))
        {
            jsonContent = reader.ReadToEnd();
        }

        TileDataListWrapper<TileData> tileData = JsonUtility.FromJson<TileDataListWrapper<TileData>>(jsonContent);
        List<TileData> list = tileData.data;
        return list;
    }
}

[System.Serializable]
public class TileDataListWrapper<T>
{
    public List<T> data;
}