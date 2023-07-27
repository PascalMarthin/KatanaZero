using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerSward : MonoBehaviour
{
    public Animator animator { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private Vector2 attackDir = Vector2.zero;


    // 타격 파워
    public float hitPower = 10;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void HitSlash(Vector3 _mousePos)
    {
        // Angle
        {
            Vector3 attackDirV3 = _mousePos - transform.position; // 방향
            attackDir = new Vector2(attackDirV3.x, attackDirV3.y);
            attackDir = attackDir.normalized;


            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // == Quaternion.Euler(0, 0, angle);


            // 캐릭터가 반대 방향을 보고 있을 경우
            if (transform.parent.right.x < 0)
            {
                spriteRenderer.flipY = true; // 이펙트의 상하 Flip
            }
            else
            {
                spriteRenderer.flipY = false;
            }
        }
        animator.SetTrigger("tAttack");
    }

    void OnDisable()
    {
        attackDir = Vector2.zero;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if(gObject.CompareTag("ENEMY"))
        {
            gObject.GetComponent<ObjectStateManager>().UnitDeath(transform.parent, attackDir, hitPower); // 플레이어 transform
        }
    }
}   
