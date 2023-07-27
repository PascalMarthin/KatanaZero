using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UnitSearchManager : MonoBehaviour
{
    // Manager
    private Unit_Nomal_StateManager stateManager;
    private UnitSightManager sightManager;
    private UnitHearmanager harmanager;


    // KnowPlayer
    public Vector2 playerPos { get; private set; }
    public bool isFind { get; private set; } = false;
    public bool isSearch { get; private set; } = false;
    public bool isSee { get; private set; } = false;
    public bool isHear { get; private set; } = false;

    void Start()
    {
        stateManager = transform.parent.GetComponent<Unit_Nomal_StateManager>();

        UnityEngine.Transform sightrange = transform.Find("SightRange");
        sightManager = sightrange.GetComponent<UnitSightManager>();

        UnityEngine.Transform hearrange = transform.Find("HearRange");
        harmanager = hearrange.GetComponent<UnitHearmanager>();
    }

    // Update is called once per frame
    void Update()
    {
        // 얻은 정보를 통합하여 계산
        if(isSee || isHear)
        {
            isFind = true;
            stateManager.FindPlayer();
        }
        else 
        {
            // stateManager Cant Find
        }
    }

    public void VisiblePlayer(Vector2 _playerPos)
    {
        isSee = true;
        playerPos = _playerPos;
    }
    public void UnvisiblePlayer()
    {
        isSee = false;
    }

    public void HearPlayer(Vector2 _playerPos)
    {
        if(isSee || isFind)
        {
            isHear = true;
            playerPos = _playerPos;
        }
    }
    public void UnHearPlayer()
    {
        isHear = false;
    }

    public void HearNoise(Vector2 _noisePos)
    {
        isSearch = true;
        playerPos = _noisePos;
        if (sightManager.IsSightIn(_noisePos) /*|| ishear*/)
        {
            stateManager.FindPlayer();
        }
        else
        {
            stateManager.Tracking(_noisePos);
        }
    }

    //
    // 근처의 적이 발견
    //
}
