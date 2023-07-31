using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class BinaryInOutput
{
    static public void SaveBinary(List<TileData> _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".bin"; // 파일명이 들어간 경로

        List<BinaryTileData> binaryData = TlieDataToBinary(_data);

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(saveFilePath, FileMode.Create))
        {
            formatter.Serialize(stream, binaryData);
        }
    }

    static public List<BinaryTileData> TlieDataToBinary(List<TileData> _data)
    {
        List < BinaryTileData > binaryTileDatas = new List < BinaryTileData >();
        foreach (TileData tile in _data)
        {
            BinaryTileData binData = new BinaryTileData();
            binData.position = new SerializableVector2Int(tile.position);
            binData.spriteName = tile.spriteName;
            binaryTileDatas.Add(binData);
        }
        return binaryTileDatas;
    }

    static public List<TileData> BinaryToTlieData(List<BinaryTileData> _data)
    {
        List<TileData> tileDatas = new List<TileData>();
        foreach (BinaryTileData tile in _data)
        {
            TileData tileData = new TileData();
            tileData.position.x = tile.position.x;
            tileData.position.y = tile.position.y;
            tileData.spriteName = tile.spriteName;
            tileDatas.Add(tileData);
        }
        return tileDatas;
    }

    static public List<TileData> LoadBinary(string _filePath)
    {
        List<BinaryTileData> data;

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(_filePath, FileMode.Open))
        {
            data = (List<BinaryTileData>)formatter.Deserialize(stream);
        }

        return BinaryToTlieData(data);
    }
}

[System.Serializable]
public class BinaryTileData
{
    public SerializableVector2Int position;
    public string spriteName;
}

[System.Serializable]
public class SerializableVector2Int
{
    public int x;
    public int y;

    public SerializableVector2Int(Vector2Int vector)
    {
        x = vector.x;
        y = vector.y;
    }
}
