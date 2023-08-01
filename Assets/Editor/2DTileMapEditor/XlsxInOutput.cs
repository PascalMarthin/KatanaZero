//using System.Collections;
//using System.Collections.Generic;
//using OfficeOpenXml;
//using UnityEngine;

//public class XlsxInOutput
//{
//    static public void SaveXlsx(List<TileData> _data, string _fileName, string _filePath)
//    {
//        string saveFilePath = _filePath;
//        saveFilePath += "/" + _fileName;
//        saveFilePath += ".xlsx";

//        string jsonStrData = JsonInOutput.TransJson(_data); // Json으로 바꾸고 Txt 저장

//        //File.WriteAllText(saveFilePath, jsonStrData);
//    }

//    static public List<TileData> LoadXlsx(string _filePath)
//    {
//        return JsonInOutput.LoadJson(_filePath);
//    }

//    public void Export(List<TileData> data, string filePath)
//    {
//        using (ExcelPackage package = new ExcelPackage())
//        {
//            // Create a new worksheet
//            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Tile Data");

//            // Add headers
//            worksheet.Cells[1, 1].Value = "ID";
//            worksheet.Cells[1, 2].Value = "Position";
//            //...

//            // Add data
//            for (int i = 0; i < data.Count; i++)
//            {
//                worksheet.Cells[i + 2, 1].Value = data[i].ID;
//                worksheet.Cells[i + 2, 2].Value = data[i].Position;
//                //...
//            }

//            // Save the Excel file
//            FileInfo fileInfo = new FileInfo(filePath);
//            package.SaveAs(fileInfo);
//        }
//    }
//}

//20230731 작업 중단 -> 라이브러리 별도 설치