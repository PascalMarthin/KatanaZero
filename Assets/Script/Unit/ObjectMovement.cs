using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    // Component
    public Rigidbody2D rb { get; private set; }

    // Movement Index
    public float moveSpeed = 2; // 기본속도
    public float movePower = 0; // 가해지는 힘(벡터)
    public float maxSpeed = 1.5f;
    

    void Start()
    {
        Setting();
    }

    public void Setting()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    //
    // ChangeFlip
    //
    public void SetFlip(float _dir)
    {
        if (_dir != 0)
        {
            if (_dir < 0)
            {
                SetFlipLeft();
            }
            else
            {
                SetFlipRight();
            }
        }
    }
    public void SetTurnFlip()
    {
        if (transform.right.x > 0)
        {
            SetFlipLeft();
        }
        else
        {
            SetFlipRight();
        }
    }
    public void SetFlipRight()
    {
        Quaternion currentQ = transform.rotation;
        transform.rotation = new Quaternion(currentQ.x, 0, currentQ.z, currentQ.w);
    }
    public void SetFlipLeft()
    {
        Quaternion currentQ = transform.rotation;
        transform.rotation = new Quaternion(currentQ.x, 180, currentQ.z, currentQ.w);
    }

    //public void SetTurnFlip()
    //{
    //    Quaternion currentQ = transform.rotation;
    //    transform.rotation = Quaternion.Euler(currentQ.x, currentQ.y + 180, currentQ.z); new Quaternion(currentQ.x, currentQ.y + 180, currentQ.z, currentQ.w);
    //}


    // Reset Velocity
    public void Resetvelocity()
    {
        rb.velocity = Vector2.zero;
    }
    public void ResetvelocityX()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    public void ResetvelocityY()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    public void SetDeath(UnityEngine.Transform _transform, Vector2 _dir, float _hitPower)
    {
        Resetvelocity();
        // 방향벡터 구하기

        SetFlip(_dir.x * -1f);
        rb.AddForce(_dir * _hitPower, ForceMode2D.Impulse);
    }

}
