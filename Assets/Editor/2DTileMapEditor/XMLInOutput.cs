using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class XMLInOutput
{
    static public void SaveXML(GridData _data, string _fileName, string _filePath)
    {
        string saveFilePath = _filePath;
        saveFilePath += "/" + _fileName;
        saveFilePath += ".xml"; // 파일명이 들어간 경로


        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridData));
        using (StreamWriter writer = new StreamWriter(saveFilePath))
        {
            xmlSerializer.Serialize(writer, _data);
        }
    }

    static public GridData LoadXML(string _filePath)
    {
        var serializer = new XmlSerializer(typeof(GridData));
        using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
        {
            GridData loadData = (GridData)serializer.Deserialize(stream);
            return loadData;
        }
    }
}
