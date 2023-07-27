using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public delegate void FunctionWithPlayerPos(Vector3 _v3);
public class Unit_Nomal_StateManager : ObjectStateManager
{
    //Component
    private UnitSearchManager searchManager;
    public BulletManager bulletManager { get; private set; }


    //
    // State
    //
    [SerializeField]
    private UnitState unitState = UnitState.Idle;
    public Function Attack_Enter { get; private set; }
    public void SetAttack_Enter(Function _f)
    {
        Attack_Enter = _f;
    }
    public FunctionWithPlayerPos Attack_Update { get; private set; }
    public void SetAttack_Update(FunctionWithPlayerPos _f)
    {
        Attack_Update = _f;
    }
    public Function Attack_End { get; private set; }
    public void SetAttack_End(Function _f)
    {
        Attack_End = _f;
    }


    enum UnitState
    {
        Idle,
        Turn,
        Patrol, // 순찰

        // Battle
        Engage, // 전투 돌입
        Tracking, // 추적

        Death,
    }

    private void Awake()
    {
        bulletManager = GameObject.Find("BulletManager").GetComponent<BulletManager>();
        searchManager = transform.GetComponentInChildren<UnitSearchManager>();
    }

    void Start()
    {
        StartSetting();


        objectFSM.CreateState("Idle", (FSM_Unit_Idle_Enter,
                                        null,
                                        FSM_Unit_Idle_Update,
                                        null,
                                        FSM_Unit_Idle_Exit));

        objectFSM.CreateState("Turn", (FSM_Unit_Turn_Enter,
                                null,
                                FSM_Unit_Turn_Update,
                                null,
                                FSM_Unit_Turn_Exit));

        objectFSM.CreateState("Patrol", (FSM_Unit_Patrol_Enter,
                                null,
                                FSM_Unit_Patrol_Update,
                                null,
                                FSM_Unit_Patrol_Exit));

        objectFSM.CreateState("Engage", (FSM_Unit_Engage_Enter,
                                null,
                                FSM_Unit_Engage_Update,
                                null,
                                FSM_Unit_Engage_Exit));

        objectFSM.CreateState("Tracking", (FSM_Unit_Tracking_Enter,
                                null,
                                FSM_Unit_Tracking_Update,
                                null,
                                FSM_Unit_Tracking_Exit));

        

        objectFSM.CreateState("Death", (FSM_Unit_Death_Enter,
                        null,
                        FSM_Unit_Death_Update,
                        null,
                        FSM_Unit_Death_Exit));

        objectFSM.ResetState("Idle");

        //objectFSM.StateChangeUpdate = FSM_Player_Update;

    }

    //
    // AI
    //

    public void FindPlayer()
    {
        if(!isDeath)
        {
            string name = objectFSM.currentState.name;
            if (name != "Turn" && name != "Engage")
            {
                objectFSM.ChangeState("Engage");
            }
        }
    }
    public void Tracking(Vector2 _playerPos)
    {

    }


    void FSM_Unit_Idle_Enter()
    {
        unitState = UnitState.Idle;
        objectAnimClass.ChangeIdleAnim();
    }
    void FSM_Unit_Idle_Update(StateInfo _info)
    {
        //TrackingPlayer();
    }
    void FSM_Unit_Idle_Exit(StateInfo _info)
    {

    }

    void FSM_Unit_Turn_Enter()
    {
        unitState = UnitState.Turn;
        objectMovement.SetTurnFlip();
        objectAnimClass.ChangeTurnAnim();
    }
    void FSM_Unit_Turn_Update(StateInfo _info)
    {
        if(objectAnimClass.currentStateInfo.normalizedTime >= 0.99f)
        {
            objectAnimClass.ChangeIdleAnim();
            objectFSM.ChangeState(_info.PostStatename); // 20230720 Debug용
        }
    }
    void FSM_Unit_Turn_Exit(StateInfo _info)
    {

    }

    void FSM_Unit_Patrol_Enter()
    {
        unitState = UnitState.Patrol;
        objectAnimClass.ChangeIdleAnim();
    }
    void FSM_Unit_Patrol_Update(StateInfo _info)
    {

    }
    void FSM_Unit_Patrol_Exit(StateInfo _info)
    {

    }
    
    void FSM_Unit_Engage_Enter()
    {
        unitState = UnitState.Engage;
        Attack_Enter();
        // objectAnimClass 추가
    }
    void FSM_Unit_Engage_Update(StateInfo _info)
    {
        if (searchManager.isFind)
        {
            Vector2 dir = searchManager.playerPos - (Vector2)transform.position;
            if (dir.x > 0 != transform.right.x > 0) // 보고있는 방향이 틀리다면
            {
                objectFSM.ChangeState("Turn");
            }
            else
            {
                Attack_Update(searchManager.playerPos); // 공격
            }
        }
        else
        {
            //objectFSM.ChangeState("Tracking"); // 시야 벗어남
        }
        //TrackingPlayer();
    }
    void FSM_Unit_Engage_Exit(StateInfo _info)
    {
        Attack_End();
    }

    //
    //
    //
    void FSM_Unit_Tracking_Enter()
    {
        unitState = UnitState.Tracking;
        //if
    }
    void FSM_Unit_Tracking_Update(StateInfo _info)
    {
        if (searchManager)
        {

        }
        else
        {

        }
        //TrackingPlayer();
    }
    void FSM_Unit_Tracking_Exit(StateInfo _info)
    {

    }

    
    void FSM_Unit_Death_Enter()
    {
        unitState = UnitState.Death;
    }
    void FSM_Unit_Death_Update(StateInfo _info)
    {

    }
    void FSM_Unit_Death_Exit(StateInfo _info)
    {

    }

}
