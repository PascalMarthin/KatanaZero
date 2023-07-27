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

    void Awake()
    {
        unitAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void ChangeTurnAnim()
    {
        if (currentStateInfo.IsName("Turn"))
        {
            Debug.Log("Same Name 'Turn'"); // 20230722 µð¹ö±× 
        }
       
        unitAnim.Play("Turn");
    }
    public void ChangeIdleAnim()
    {
        unitAnim.Play("Idle");
    }
    
    public void ChangeAnimstr(string _aimName)
    {
        unitAnim.Play(_aimName);
    }
}
