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

    // 20230717 ���� ���� : PlayerState�� ��ü,
    // ����Ȯ���ϴ� ������ ������ ������ �����ϰ� ������� ���� �߻��� ���ɼ��� ����
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

        // 20230717 Ʃ�� �׸��̸� ��ٰ� ���� ����


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
    // �������� ������ �ٲ� ���� �ֱ� ����
    // ������ ���¸� üũ �ϴ� ��
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

    // ���� ����

    void SetJump()
    {
        playerMovement.SetJump();
        //objectFSM.ChangeState("Aerial");
        Vector2 landPos = raycastInfo.point;
        playerEffectManagerClass.PlayJumpCloudEffect(transform.position);
    }

    // CheckState
    // �� ���� ������Ʈ ���� �����ϸ� ���¿� ���� üũ�ؾ��ϴ� �Է��� Ȯ�����ش�
    // �޼ҵ�� �����ϴ� ���°� �������� ������ ���� �� ���Ƽ� �ϳ��� �޼ҵ�� switch���� ����Ͽ�
    // �������� ���������� �����ϰ� ��
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
                    // �ϳ��� state�� �ٲ�ٸ� ��� ��ȯ
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
                    if (IsAerial()) // ü�� �����ΰ�?
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if (Input_Move()) // �޸��� ���ΰ�?
                    {
                        if (Input_Roll()) // ũ���ġ Ű�� ���ȴ°�?
                        {
                            objectFSM.ChangeState("Roll");
                            isChangeState = true;
                        }
                        if (Input_Jump()) // ���� ���ΰ�?
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
                        // ���� ����
                        playerEffectManagerClass.PlayLandCloudEffect(transform.position);


                        objectFSM.ChangeState("Idle");
                        isChangeState = true;
                    }
                    // �� ������ �޼ҵ� �߰�
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
                    // ���� �Ұ�
                    if (IsAerial()) // ü�� �����ΰ�?
                    {
                        objectFSM.ChangeState("Aerial");
                        isChangeState = true;
                    }
                    else if(!Input_Crouch())
                    {
                        objectFSM.ChangeState("Idle");
                        isChangeState = true;
                    }
                    else if (/*Input_Move()*/Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) // �޸��� ���ΰ�?
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
                        if (animRate >= 1 || isGround) // �ð��� ������ ĵ�� || �� ������ ���� �����ϸ� ĵ��
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
    /// FSM�� �޼ҵ��
    /// </summary>
    /// 

    // ����

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
