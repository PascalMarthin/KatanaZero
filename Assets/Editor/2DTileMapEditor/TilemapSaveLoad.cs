using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaveLoad
{

    static private Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>(); // 스프라이트 저장
    static public void ClearSpriteDictionary()
    {
        spriteDictionary.Clear(); // 저장된 스프라이트 제거
    }


    static public List<TileData> SaveTilemap(Tilemap _targetTilemap)
    {
        var bounds = _targetTilemap.cellBounds;
        var allTiles = _targetTilemap.GetTilesBlock(bounds);

        List<TileData> tileDataList = new List<TileData>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int z = 0; z < bounds.size.z; z++)
                {
                    var localPlace = new Vector3Int(x, y, z) + bounds.min; // 최소값으로 기준 이동
                    TileBase tileBase = allTiles[x + y * bounds.size.x];
                    Tile tile = tileBase as Tile;
                    if (tile != null) // 타일이 있는 경우에만 리스트에 추가
                    {
                        var tileData = new TileData { position = (Vector2Int)localPlace, spriteName = tile.sprite.name };
                        tileDataList.Add(tileData);
                    }
                }
            }
        }
        return tileDataList;
    }

    public static TilemapData MakeTileData(Tilemap _tilemap)
    {
        List<TileData> tileDatas = TilemapSaveLoad.SaveTilemap(_tilemap); // 타일맵 저장
        TilemapData tilemapData = new TilemapData();
        tilemapData.tileDatas = tileDatas;
        tilemapData.tilemapName = _tilemap.name;
        tilemapData.localPosition = _tilemap.gameObject.transform.position;
        return tilemapData;
    }

    // 일반 타일맵 그리기
    static public Tilemap MakeNomalTilemap(List<TileData> _data, string _fileName)
    {
        if (_fileName == "")
        {
            _fileName = "Default";
        }

        GameObject tilemapGameObject = new GameObject(_fileName);

        // Tilemap 컴포넌트와 Tilemap Renderer 컴포넌트를 게임 오브젝트에 추가
        Tilemap inputTileMap = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapGameObject.GetComponent<TilemapRenderer>().sortingLayerName = "BACKGROUND"; // 레이어 변경



        // 데이터를 TileMap에다가 적용
        foreach (TileData tileData in _data)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            string tileSpritesStr = tileData.spriteName;

            // 스프라이트 배열에서 이름으로 스프라이트를 찾음
            Sprite sprite;
            if (!spriteDictionary.TryGetValue(tileSpritesStr, out sprite))
            {
                // 없으면 Load
                string folderStr = "Sprites/Map/Bunker/";
                int index = tileSpritesStr.LastIndexOf('_');
                Sprite[] sprites;
                if (index != -1)
                {
                    folderStr += tileSpritesStr.Substring(0, index);
                    sprites = Resources.LoadAll<Sprite>(folderStr);
                    if (sprites.Length > 0)
                    {
                        foreach (Sprite spr in sprites)
                        {
                            spriteDictionary.Add(spr.name, spr);
                        }
                    }
                    else
                    {
                        Debug.Log("We cant find sprites Folder -> " + tileSpritesStr);
                        continue;
                    }

                    if (!spriteDictionary.TryGetValue(tileSpritesStr, out sprite))
                    {
                        Debug.Log("We cant find sprite -> " + tileSpritesStr);
                        continue;
                    }
                }
                else
                {
                    Debug.Log("This sprite is not allow -> " + tileSpritesStr);
                    continue;
                }
            }
            // 타겟 폴더 내 리소스 가져오기
            tile.sprite = sprite;

            // 타일을 타일맵에 적용함
            inputTileMap.SetTile(new Vector3Int(tileData.position.x, tileData.position.y, 0), tile);
        }

        return inputTileMap;
    }

    // 알파 맵 그리기
    static public Tilemap MakeAlphaTilemap(List<TileData> _data, string _fileName)
    {
        if (_fileName == "")
        {
            _fileName = "Default";
        }

        GameObject tilemapGameObject = new GameObject(_fileName + "_alpha");

        // Tilemap 컴포넌트와 Tilemap Renderer 컴포넌트를 게임 오브젝트에 추가
        Tilemap inputTileMap = tilemapGameObject.AddComponent<Tilemap>();
        tilemapGameObject.AddComponent<TilemapRenderer>();
        tilemapGameObject.GetComponent<TilemapRenderer>().sortingLayerName = "BACKGROUND_ALPHA"; // 레이어 변경


        // 데이터를 TileMap에다가 적용
        foreach (TileData tileData in _data)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();

            StringBuilder sb = new StringBuilder(tileData.spriteName); // 조합할 원본 문자열

            int lastIndex = tileData.spriteName.LastIndexOf('_');
            int secondLastIndex = tileData.spriteName.LastIndexOf('_', lastIndex - 1);

            sb.Insert(secondLastIndex, "_alpha");

            string tileSpritesStr = sb.ToString();


            // 스프라이트 배열에서 이름으로 스프라이트를 찾음
            Sprite sprite;
            if (!spriteDictionary.TryGetValue(tileSpritesStr, out sprite))
            {
                // 없으면 Load
                string folderStr = "Sprites/Map/Bunker/";
                int index = tileSpritesStr.LastIndexOf('_');
                Sprite[] sprites;
                if (index != -1)
                {
                    folderStr += tileSpritesStr.Substring(0, index);
                    sprites = Resources.LoadAll<Sprite>(folderStr);
                    if (sprites.Length > 0)
                    {
                        foreach (Sprite spr in sprites)
                        {
                            spriteDictionary.Add(spr.name, spr);
                        }
                    }
                    else
                    {
                        Debug.Log("We cant find sprites Folder -> " + tileSpritesStr);
                        continue;
                    }

                    if (!spriteDictionary.TryGetValue(tileSpritesStr, out sprite))
                    {
                        Debug.Log("We cant find sprite -> " + tileSpritesStr);
                        continue;
                    }
                }
                else
                {
                    Debug.Log("This sprite is not allow -> " + tileSpritesStr);
                    continue;
                }
            }
            // 타겟 폴더 내 리소스 가져오기
            tile.sprite = sprite;

            // 타일을 타일맵에 적용함
            inputTileMap.SetTile(new Vector3Int(tileData.position.x, tileData.position.y, 0), tile);
        }

        return inputTileMap;
    }

    public enum TilemapType
    {
        ect,
        Wall,
        BackGround
    }

    // 반환은 GameObject로 안하고 Tilemap으로 한 이유
    // 명시적으로 표시하고 싶어서

    // frist Nomal, second Alpha
    public static Tuple<Tilemap, Tilemap> MakeTilemap(TilemapData _tilemap, TilemapType _type)
    {

        List<TileData> allTilesDatas = _tilemap.tileDatas;
        string tilemapName = _tilemap.tilemapName;
        Vector3 tilemapLocalPos = _tilemap.localPosition;

        Tilemap nomalTilemap = MakeNomalTilemap(allTilesDatas, tilemapName);
        nomalTilemap.gameObject.transform.localPosition = tilemapLocalPos;
        if (_type == TilemapType.Wall)
        {
            TilemapCollider2D tilemapCollider2D = nomalTilemap.gameObject.AddComponent<TilemapCollider2D>();
            tilemapCollider2D.usedByComposite = true;
            nomalTilemap.gameObject.AddComponent<CompositeCollider2D>();
        }

        Tilemap alphaTilemap = MakeAlphaTilemap(allTilesDatas, tilemapName);
        nomalTilemap.gameObject.transform.localPosition = tilemapLocalPos;

        var pair = new Tuple<Tilemap, Tilemap>(nomalTilemap, alphaTilemap);

        return pair;
    }
}

[System.Serializable]
public class TilemapData 
{
    public string tilemapName;
    public Vector3 localPosition;

    public List<TileData> tileDatas = new List<TileData>();
}
