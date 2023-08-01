using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// ���¸� �����ϰ� �����ϴ� ������
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

    // �ڽ� ��ü���� Start
    public void StartSetting() 
    {
        objectMovement = GetComponent<ObjectMovement>();
        objectFSM = GetComponent<FSManager>();
        objectAnimClass = GetComponent<ObjectAnimation>();
        ccollider = GetComponent<CircleCollider2D>();

        rayDistance = GetComponent<CircleCollider2D>().radius + 0.01f; // ������ ��ŭ ����
        groundLayer = PlayManager.wallLayer;
    }

    void Update()
    {
        CheckGround();

        // isDead :: ���� ���� ����� �ٴ����� �ƴ��� Ȯ�� �� �ʿ䰡 ������
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

            // �ڷ�ƾ
            //PlayManager
            PlayManager.Inst.PlayKillSlow();
        }
    }
}
