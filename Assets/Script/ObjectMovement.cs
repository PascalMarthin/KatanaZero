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

    void FixedUpdate()
    {

         //transform.Translate(Vector2.right * moveSpeed * movePower * PlayManager.gameFixedTime);
        
    }

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
        rb.AddForce(_dir * _hitPower, ForceMode2D.Impulse);
    }

}
