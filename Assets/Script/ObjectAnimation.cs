using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimation : MonoBehaviour
{
    // Component
    public Animator unitAnim { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }


    public AnimatorStateInfo currentStateInfo 
    {
        get
        {
            return unitAnim.GetCurrentAnimatorStateInfo(0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        unitAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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


    //
    ///////////////////////////////////////
    //

    public void SendParameterisGround(bool _isGround)
    {
        unitAnim.SetBool("isGround", _isGround);
    }

    public void ChangeDeathAnim()
    {
        unitAnim.Play("Death_Aerial");
    }

}
