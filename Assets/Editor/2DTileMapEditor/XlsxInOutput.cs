using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XlsxInOutput
{
    static public void SaveXlsx(List<TileData> _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".xlsx";

        string jsonStrData = JsonInOutput.TransJson(_data); // Json으로 바꾸고 Txt 저장

        File.WriteAllText(saveFilePath, jsonStrData);
    }

    static public List<TileData> LoadXlsx(string _filePath)
    {
        return JsonInOutput.LoadJson(_filePath);
    }
}
