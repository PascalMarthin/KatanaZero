using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using static FSManager;

public class PlayerStateManager : ObjectStateManager
{
    // Componenet
    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;
    private PlayerEffectManager playerEffectManagerClass;
    public PlayerSward playerSwardClass;

    // State
    [SerializeField]
    private PlayerState playerState = PlayerState.Idle;

    // Input_Key
    private KeyCode keyCrouch = KeyCode.S;
    private KeyCode keyJump = KeyCode.W;

    // Input Power
    private float xAxis = 0;

    // Current
    private ParticleSystem currentPS;

    // 20230717 제거 이유 : PlayerState로 대체,
    // 상태확인하는 변수가 많으면 오히려 복잡하고 예상못한 예외 발생할 가능성이 높음
    //public bool isRolling { get; private set; } = false;
    //public bool isRun { get; private set; } = false;

    // 


    enum PlayerState
    {
        Idle
        , Run
        , Aerial
        , Roll
        , Crouch
        , Climb
        , Attack
    }

    void Start()
    {
        StartSetting();

        playerMovement = ((PlayerMovement)objectMovement);
        playerAnimation = ((PlayerAnimation)objectAnimClass);
        playerEffectManagerClass = GetComponent<PlayerEffectManager>();

        // 20230717 튜플 항목이름 썼다가 버그 생김


        objectFSM.CreateState("Idle", (FSM_Player_Idle_Enter,
                                        null,
                                        FSM_Player_Idle_Update,
                                        null,
                                        FSM_Player_Idle_Exit));

        objectFSM.CreateState("Run", (FSM_Player_Run_Enter,
                                       FSM_Player_FixedUpdate,
                                        FSM_Player_Run_Update,
                                        null,
                                        FSM_Player_Run_Exit));

        objectFSM.CreateState("Aerial", (FSM_Player_Aerial_Enter,
                                        FSM_Player_FixedUpdate,
                                        FSM_Player_Aerial_Update,
                                        null,
                                        FSM_Player_Aerial_Exit));

        objectFSM.CreateState("Roll", (FSM_Player_Roll_Enter,
                                        null,
                                        FSM_Player_Roll_Update,
                                        null,
                                        FSM_Player_Roll_Exit));

        objectFSM.CreateState("Crouch", (FSM_Player_Crouch_Enter,
                                        null,
                                        FSM_Player_Crouch_Update,
                                        null,
                                        FSM_Player_Crouch_Exit));

        objectFSM.CreateState("Climb", (FSM_Player_Climb_Enter,
                                        null,
                                        FSM_Player_Climb_Update,
                                        null,
                                        FSM_Player_Climb_Exit));

        objectFSM.CreateState("Attack", (FSM_Player_Attack_Enter,
                                        null,
                                        FSM_Player_Attack_Update,
                                        null,
                                        FSM_Player_Attack_Exit));

        objectFSM.ResetState("Idle");

        objectFSM.StateChangeUpdate = FSM_Player_Update;
    }

    //void Update()
    //{
    //    CheckGround();
    //    Input_Horizontal();
    //}

    // Input
    // 언제든지 기준이 바뀔 수도 있기 때문
    // 본질은 상태를 체크 하는 것
    void SendMovePower()
    {
        Input_Horizontal();
        playerMovement.xAxis = Mathf.Abs(xAxis);
    }

    void Input_Horizontal()
    {
        xAxis = Input.GetAxis("Horizontal");
    }

    
    bool Input_Jump()
    {
        bool isInputJump = false;
        if (Input.GetKeyDown(keyJump))
        {
            isInputJump = true;
        }
        return isInputJump;
    }

    bool Input_Move()
    {
        bool isInputMove = false;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            isInputMove = true;
        }
        return isInputMove;
    }
    bool Input_Crouch()
    {
        bool isInputCrouch = false;
        if (Input.GetKey(keyCrouch))
        {
            isInputCrouch = true;
        }
        return isInputCrouch;
    }
    bool Input_Roll()
    {
        bool isInputCrouch = false;
        if (Input.GetKeyDown(keyCrouch))
        {
            isInputCrouch = true;
        }
        return isInputCrouch;
    }
    bool Input_Attack()
    {
        bool isInputAttack = false;
        if (Input.GetMouseButtonDown(0))
        {
            isInputAttack = true;
        }
        return isInputAttack;
    }
    bool IsAerial()
    {
        bool isChangeState = false;
        if (!isGround)
        {
            isChangeState = true;
        }
        return isChangeState;
    }

    // 상태 변경

    void SetJump()
    {
        playerMovement.SetJump();
        //objectFSM.ChangeState("Aerial");
        Vector2 landPos = raycastInfo.point;
        playerEffectManagerClass.PlayJumpCloudEffect(transform.position);
    }

    // CheckState
    // 매 상태 업데이트 마다 실행하며 상태에 따라 체크해야하는 입력을 확인해준다
    // 메소드로 구분하니 상태가 많이지면 관리가 힘들 것 같아서 하나의 메소드로 switch문을 사용하여
    // 가독성과 유지보수를 용이하게 함
    bool CheckChangeOtherState(PlayerState _CurrentState)
    {
        bool isChangeState = false;

        if(Input_Attack())
        {
            objectFSM.ChangeState("Attack");
            isChangeState = true;
            return isChangeState;
        }

        switch (_CurrentState)
        {
            case PlayerState.Idle:
                {
                    // 하나라도 state가 바뀐다면 즉시 반환
                    if (IsAerial())
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if(Input_Move())
                    {
                        objectFSM.ChangeState("Run");
                        isChangeState = true;
                    }
                    else if(Input_Jump())
                    {
                        SetJump();
                        isChangeState = true;
                    }
                    else if(Input_Crouch())
                    {
                        objectFSM.ChangeState("Crouch");
                        isChangeState = true;
                    }

                }
                break;
            case PlayerState.Run:
                {
                    if (IsAerial()) // 체공 상태인가?
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if (Input_Move()) // 달리는 중인가?
                    {
                        if (Input_Roll()) // 크라우치 키가 눌렸는가?
                        {
                            objectFSM.ChangeState("Roll");
                            isChangeState = true;
                        }
                        if (Input_Jump()) // 점프 중인가?
                        {
                            SetJump();
                            isChangeState = true;
                        }
                    }
                    else
                    {
                        objectFSM.ChangeState("Idle");
                        isChangeState = true;
                    }
                }
                break;
            case PlayerState.Aerial:
                {
                    if (isGround)
                    {
                        // 먼지 생성
                        playerEffectManagerClass.PlayLandCloudEffect(transform.position);


                        objectFSM.ChangeState("Idle");
                        isChangeState = true;
                    }
                    // 벽 붙을때 메소드 추가
                }
                break;
            case PlayerState.Roll:
                {
                    if (IsAerial())
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if (Input_Jump())
                    {
                        SetJump();
                        isChangeState = true;
                    }
                    else if (playerAnimation.currentStateInfo.normalizedTime >= 1)
                    {
                        objectFSM.ChangeState("Crouch");
                        playerAnimation.ChangeCrouchAnim();
                        isChangeState = true;
                    }
                }
                break;
            case PlayerState.Crouch:
                {
                    // 점프 불가
                    if (IsAerial()) // 체공 상태인가?
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if(!Input_Crouch())
                    {
                        objectFSM.ChangeState("Idle");
                        isChangeState = true;
                    }
                    else if (/*Input_Move()*/Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) // 달리는 중인가?
                    {
                        objectFSM.ChangeState("Roll");
                        isChangeState = true;
                    }
                }
                break;
            case PlayerState.Climb:
                break;
            case PlayerState.Attack:
                {
                    float animRate = playerAnimation.currentStateInfo.normalizedTime;
                    if (animRate > 0.8f) 
                    {
                        if (animRate >= 1 || isGround) // 시간이 지나면 캔슬 || 후 딜레이 동안 착지하면 캔슬
                        {
                            objectFSM.ChangeState("Aerial");
                        }
                    }
                }

                break;
            default:
                Debug.Log("It is Not registrate Enum");
                break;
        }
        return isChangeState;
    }

    void SendParameter()
    {
        playerAnimation.SendParameterisGround(isGround);
        playerAnimation.SendParameterisRun(playerState == PlayerState.Run);
        playerAnimation.SendParameterisCrouch(playerState == PlayerState.Crouch);
        playerAnimation.SendParameterisRoll(playerState == PlayerState.Roll);
        playerAnimation.SendParameterfYPower(playerMovement.yVelocity);
    }


    // CallBack
    //void CallBack_Aim_Roll_End()
    //{
    //    if (Input_Move())
    //    {
    //        objectFSM.ChangeState("Run");
    //    }
    //    else if (Input_Crouch())
    //    {
    //        objectFSM.ChangeState("Crouch");
    //    }
    //    else
    //    {
    //        objectFSM.ChangeState("Idle");
    //    }
    //}

    //void CallBack_Aim_Attack_End()
    //{
    //     objectFSM.ChangeState("Aerial");
    //}

    /// <summary>
    /// FSM용 메소드들
    /// </summary>
    /// 

    // 공통

    void FSM_Player_FixedUpdate(StateInfo _info)
    {
        playerMovement.PlayerMove();
    }
    void FSM_Player_LateUpdate(StateInfo _info)
    {
        //playerAnimation.SetFlip(xAxis);
        //CheckChangeOtherState(playerState);
        //SendParameter();
    }
    void FSM_Player_Update()
    {
        playerAnimation.SetFlip(xAxis);
        CheckChangeOtherState(playerState);
        SendParameter();
    }


    // Idle
    void FSM_Player_Idle_Enter()
    {
        playerState = PlayerState.Idle;
    }
    void FSM_Player_Idle_Update(StateInfo _info)
    {
        SendMovePower();
    }
    void FSM_Player_Idle_Exit(StateInfo _info)
    {

    }


    // Run
    void FSM_Player_Run_Enter()
    {
        playerState = PlayerState.Run;
        playerAnimation.SetFlip(xAxis);
        currentPS = playerEffectManagerClass.PlayRunCloudEffect();

    }
    void FSM_Player_Run_Update(StateInfo _info)
    {
        SendMovePower();
    }
    void FSM_Player_Run_Exit(StateInfo _info)
    {
        if(currentPS)
        {
            playerEffectManagerClass.StopRunCloudEffect(currentPS);
        }
        currentPS = null;
    }

    // Jump
    void FSM_Player_Aerial_Enter()
    {
        playerState = PlayerState.Aerial;
        playerAnimation.ChangeJumpAnim();
    }
    void FSM_Player_Aerial_Update(StateInfo _info)
    {
        SendMovePower();
    }                            
    void FSM_Player_Aerial_Exit(StateInfo _info)
    {
        
    }

    // Roll
    void FSM_Player_Roll_Enter()
    {
        playerState = PlayerState.Roll;

        float xDir = Input.GetAxisRaw("Horizontal");
        // ChangeDir
        playerAnimation.SetFlip(xDir);
        playerAnimation.ChangeRollAnim();
        
        playerMovement.SetRolling();
    }
    void FSM_Player_Roll_Update(StateInfo _info)
    {

    }
    void FSM_Player_Roll_Exit(StateInfo _info)
    {

    }

    // Crouch
    void FSM_Player_Crouch_Enter()
    {
        playerState = PlayerState.Crouch;
        playerAnimation.SendParameterisCrouch(playerState == PlayerState.Crouch);
    }
    void FSM_Player_Crouch_Update(StateInfo _info)
    {

    }
    void FSM_Player_Crouch_Exit(StateInfo _info)
    {

    }

    // Climb
    void FSM_Player_Climb_Enter()
    {
        playerState = PlayerState.Climb;
    }
    void FSM_Player_Climb_Update(StateInfo _info)
    {

    }
    void FSM_Player_Climb_Exit(StateInfo _info)
    {

    }

    // Attack
    void FSM_Player_Attack_Enter()
    {
        playerState = PlayerState.Attack;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerMovement.SetAttack(mousePos);

        xAxis = 0;
        // ChangeDir
        playerAnimation.ChangeAttackAnim();
        playerAnimation.SetFlip(mousePos.x - transform.position.x);

        // Sward Active
        playerSwardClass.gameObject.SetActive(true);
        playerSwardClass.HitSlash(mousePos);


        //playerAnimation.SetSlash();
    }
    void FSM_Player_Attack_Update(StateInfo _info)
    {
        
    }
    void FSM_Player_Attack_Exit(StateInfo _info)
    {
        playerSwardClass.gameObject.SetActive(false);
    }

}
