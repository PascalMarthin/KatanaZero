using System.Collections.Generic;
using System.IO;

public class TXTInOutput
{
    static public void SaveTxt(List<TileData> _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".txt";

        string jsonStrData = JsonInOutput.TransJson(_data); // Json���� �ٲٰ� Txt ����

        File.WriteAllText(saveFilePath, jsonStrData);
    }

    static public List<TileData> LoadTxt(string _filePath)
    {
        return JsonInOutput.LoadJson(_filePath);
    }
}
