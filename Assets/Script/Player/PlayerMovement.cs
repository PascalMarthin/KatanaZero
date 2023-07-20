using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// 플레이어의 움직임을 총괄하는 클래스
public class PlayerMovement : ObjectMovement
{



    //
    // Movement Index
    //

    public float jumpPower = 4;
    public float rollingSpeed = 5;
    public float attackRange = 5;
    public Vector2 minattackRange;
    public float xAxis = 0;

    public float yVelocity 
    {
        get { return rb.velocity.y; }
    }

    void Start()
    {
        Setting();
    }

    void FixedUpdate()
    {

    }

    public void PlayerMove()
    {
        xAxis *= 2f;
        if (xAxis > 1)
        {
            xAxis = 1;
        }

        rb.AddForce(transform.right * xAxis * moveSpeed, ForceMode2D.Force);

        Vector2 currentVelocity = rb.velocity;

        if (currentVelocity.x > maxSpeed)
        {
            currentVelocity.x = maxSpeed;
        }
        else if (currentVelocity.x < -maxSpeed)
        {
            currentVelocity.x = maxSpeed * -1;
        }

        
        rb.velocity = currentVelocity;

    }


    // Update is called once per frame


    // ----------------------------------
    public void SetJump()
    {
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    // Roll
    public void SetRolling()
    {
        // rb
        ResetvelocityX();
        rb.AddForce(transform.right * rollingSpeed, ForceMode2D.Impulse);
    }
    // Attack
    public void SetAttack(Vector3 _MousePos)
    {
        // velocity 조절
        rb.velocity = new Vector2(0, rb.velocity.y / 2);


        Vector3 attackDirV3 = _MousePos - transform.position; // 방향
        Vector2 attackDirV2 = new Vector2(attackDirV3.x, attackDirV3.y);
        
        Vector2 RealRange = attackDirV2.normalized * attackRange;
        if(RealRange.x < 0)
        {
            RealRange.y += minattackRange.y;
            RealRange.x += minattackRange.x * -1;
        }
        else
        {
            RealRange += minattackRange;
        }
        
        rb.AddForce(RealRange, ForceMode2D.Impulse);
    }

}
