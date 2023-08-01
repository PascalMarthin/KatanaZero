using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// 상태를 설정하고 전달하는 관리자
public class ObjectStateManager : MonoBehaviour
{
    // State
    public bool isGround { get; private set; } = false;
    public bool isInvincibility { get; private set; } = false;
    public bool isDeath = false;
    public Vector2 deathOffSet = Vector2.zero;

    // Class
    public ObjectMovement objectMovement { get; private set; }
    public FSManager objectFSM { get; private set; }
    public ObjectAnimation objectAnimClass { get; private set; }
    public CircleCollider2D ccollider { get; private set; }

    // RayDistance
    private float rayDistance = 1.0f;
    public LayerMask groundLayer { get; private set; }

    public RaycastHit2D raycastInfo { get; private set; }



    void Start()
    {
        StartSetting();
    }

    // 자식 객체에서 Start
    public void StartSetting() 
    {
        objectMovement = GetComponent<ObjectMovement>();
        objectFSM = GetComponent<FSManager>();
        objectAnimClass = GetComponent<ObjectAnimation>();
        ccollider = GetComponent<CircleCollider2D>();

        rayDistance = GetComponent<CircleCollider2D>().radius + 0.01f; // 반지름 만큼 설정
        groundLayer = PlayManager.wallLayer;
    }

    void Update()
    {
        CheckGround();

        // isDead :: 죽은 상태 말고는 바닥인지 아닌지 확인 할 필요가 없으니
        if(isDeath)
        {
            objectAnimClass.SendParameterisGround(isGround);
        }
        
    }

    public void CheckGround()
    {
        raycastInfo = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
        if (raycastInfo)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public virtual void UnitDeath(UnityEngine.Transform _transform, Vector2 _dir, float _hitPower)
    {
        //if(!isDeath)
        {
            isDeath = true;
            ccollider.offset = deathOffSet;

            objectFSM.ChangeState("Death");
            objectMovement.SetDeath(_transform, _dir, _hitPower);
            objectAnimClass.ChangeDeathAnim();

            // 코루틴
            //PlayManager
            PlayManager.Inst.PlayKillSlow();
        }
    }
}
