using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageSaveLoad 
{
    static public StageData MakeStageData(GameObject _object)
    {
        if(_object.transform.childCount < 1)
        {
            return null;
        }


        StageData stageData = new StageData();
        stageData.stageName = _object.name;
        stageData.localStagePosition = _object.transform.localPosition;

        List<GameObject> childObjects = new List<GameObject>();
        for (int i = 0; i < _object.transform.childCount; i++)
        {
            childObjects.Add(_object.transform.GetChild(i).gameObject);
        }

        foreach(GameObject obj in childObjects)
        {
            Tilemap tilemap = obj.GetComponent<Tilemap>();
            if (tilemap)
            {
                TilemapData tilemapData = TilemapSaveLoad.MakeTileData(tilemap);

                if (tilemapData.tilemapName.Contains("_Wall"))
                {
                    stageData.wallTilemap = tilemapData;
                }
                else if (tilemapData.tilemapName.Contains("_BackGround"))
                {
                    stageData.backGroundTilemap = tilemapData;
                }
                else
                {
                    stageData.ectTilemap = tilemapData;
                    Debug.Log("This tilemap name is wrong -> " + tilemapData.tilemapName);
                }
            }
            else // Obj 일 경우
            {
                // 추후 제작
            }
        }
        return stageData;
    }

    static public GameObject MakeStage(StageData _stageData)
    {
        if(_stageData == null)
        {
            return null;
        }

        GameObject stage = new GameObject();
        stage.name = _stageData.stageName;                                                                  
        stage.transform.localPosition = _stageData.localStagePosition;


        var wallTilemap = _stageData.wallTilemap;
        var backGroundTilemap = _stageData.backGroundTilemap;

        foreach (var obj in _stageData.objectDatas)
        {
            // 추가
        }

        Tuple<Tilemap, Tilemap> alphanomalTilemaps = null;
        if (_stageData.wallTilemap != null)
        {
            alphanomalTilemaps = TilemapSaveLoad.MakeTilemap(wallTilemap, TilemapSaveLoad.TilemapType.Wall);
            alphanomalTilemaps.Item1.gameObject.transform.SetParent(stage.transform);
            alphanomalTilemaps.Item2.gameObject.transform.SetParent(stage.transform);
        }
        if (_stageData.backGroundTilemap != null)
        {
            alphanomalTilemaps = TilemapSaveLoad.MakeTilemap(backGroundTilemap, TilemapSaveLoad.TilemapType.Wall);
            alphanomalTilemaps.Item1.gameObject.transform.SetParent(stage.transform);
            alphanomalTilemaps.Item2.gameObject.transform.SetParent(stage.transform);
        }

        return stage;
    }
}

[System.Serializable]
public class StageData
{
    public string stageName;
    public Vector3 localStagePosition;

    public TilemapData wallTilemap;
    public TilemapData backGroundTilemap;
    public TilemapData ectTilemap;

    public List<ObjectData> objectDatas = new List<ObjectData>();
}

