using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSaveLoad
{
    static public GridData MakeGridData(Grid _grid)
    {
        GridData grid = new GridData();

        GameObject gridGameObject = _grid.gameObject;
        {
            for(int i = 0;i < gridGameObject.transform.childCount; i++)
            {
                StageData statgeData = StageSaveLoad.MakeStageData(gridGameObject.transform.GetChild(i).gameObject);
                if (statgeData != null)
                {
                    grid.stageDatas.Add(statgeData);
                }
            }
        }
        return grid;
    }

    static public GameObject MakeGrid(GridData _grid)
    {
        if (_grid == null)
        {
            return null;
        }

        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            grid = new GameObject("Grid");
            grid.AddComponent<Grid>();
        }

        foreach(StageData stagedata in _grid.stageDatas)
        {
            GameObject obj = StageSaveLoad.MakeStage(stagedata);
            obj.transform.SetParent(grid.transform);
        }

        return grid;
    }
}


[System.Serializable]
public class GridData
{
    public List<StageData> stageDatas = new List<StageData>();
}


// 프리팹으로 저장
[System.Serializable]
public class ObjectData
{
    public string objectName;
    public string prefabName;

    public Vector3 localPosition;
    public Vector3 localLotate;
    public Vector3 localScale;

}