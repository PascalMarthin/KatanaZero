using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ִϸ��̼��� �����ϴ� Ŭ����
public class PlayerAnimation : ObjectAnimation
{

    // Start is called before the first frame update

    //
    // SettingParameter
    //
    public void SendParameterisCrouch(bool _isCrouch)
    {
        unitAnim.SetBool("isCrouch", _isCrouch);
    }
    public void SendParameterisRoll(bool _isRoll)
    {
        unitAnim.SetBool("isRoll", _isRoll);
    }
    public void SendParameterisRun(bool _isRun)
    {
        unitAnim.SetBool("isRun", _isRun);
    }
    public void SendParameterfYPower(float _power)
    {
        unitAnim.SetFloat("fYPower", _power);
    }
    public void SendParameterfXPower(float _power)
    {
        unitAnim.SetFloat("fXPower", _power);
    }
    public void SendParameterfXInPut(float _input)
    {
        unitAnim.SetFloat("fXInPut", _input);
    }
    

    // SetTrigger - ���������� �ٲٴ� �޼ҵ�
    public void SetRollAnim()
    {
        unitAnim.SetTrigger("tRoll");
    }

    // ���������� �ٲٴ� �޼ҵ�
    public void ChangeJumpAnim()
    {
        unitAnim.Play("Player_Jump");
    }
    public void ChangeRollAnim()
    {
        unitAnim.Play("Player_Roll");
    }
    public void ChangeAttackAnim()
    {
        unitAnim.Play("Player_Attack");
    }
    public void ChangeCrouchAnim()
    {
        unitAnim.Play("Player_Crouch");
    }

}
