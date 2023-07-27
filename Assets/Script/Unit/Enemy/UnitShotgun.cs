using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class UnitShotgun : MonoBehaviour
{
    Unit_Nomal_StateManager unitStateManager;

    // ÆÈ
    public GameObject arm; // armOffset = Vector2(0.071f, 0.03f);
    public float rotateSpeed = 100;
    public float frameDelay = 0.1f;
    public float currnetFrameDelay = 0;

    // ÇÁ·¹ÀÓ


    // ÃÑ
    public int shotgunPallet = 5; // ¼¦°Ç Æç¸´ ¼ö
    public float spreadAngle = 30f;
    public float shootDelayTime = 1;
    public float shootAfterDelay = 1.7f;
    private float shootDelay = 0;
    public Vector2 shootPoint = new Vector2(0.345f, 0.038f); // »ç°Ý ÁöÁ¡

    //

    public GameObject shell;// ÅºÇÇ
    public Vector2 maxShellPower = new Vector2(1.5f, 1.5f); // ÅºÇÇ Æ¥ ¶§ Èû
    public Vector2 minShellPower = new Vector2(1, 1); // ÅºÇÇ Æ¥ ¶§ Èû

    private void Start()
    {
        unitStateManager = GetComponent<Unit_Nomal_StateManager>();
        unitStateManager.SetAttack_Enter(Aim_Enter);
        unitStateManager.SetAttack_Update(WaitforShot);
        unitStateManager.SetAttack_End(Aim_Exit);
        shootDelay = 0;
    }

    void Aim_Enter()
    {
        arm.transform.localRotation = Quaternion.Euler(0, 0, 0);

        currnetFrameDelay = 0;

    }

    void WaitforShot(Vector3 _pos)
    {
        shootDelay += PlayManager.gameDeltaTime;
        if (shootDelay > shootAfterDelay)
        {
            Aimming(_pos);
        }
        else if(shootDelay > 0)
        {
            unitStateManager.objectAnimClass.ChangeIdleAnim();
            arm.SetActive(false);
        }

    }

    void Aimming(Vector3 _pos)
    {
        unitStateManager.objectAnimClass.ChangeAnimstr("Unit_Shotgun_Aim");
        arm.SetActive(true);
        ReadyforShoot(_pos);
    }

    void ReadyforShoot(Vector3 _pos)
    {
        currnetFrameDelay += PlayManager.gameDeltaTime;
        if(currnetFrameDelay >= frameDelay)
        {
            _pos.y += 0.05f;
            Vector2 direction = _pos - arm.transform.position;
            if(transform.right.x < 0)
            {
                direction = Quaternion.Inverse(transform.rotation) * direction;
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Vector3 currentRotation = arm.transform.rotation.eulerAngles;
            arm.transform.rotation = Quaternion.RotateTowards(arm.transform.rotation, Quaternion.Euler(currentRotation.x, currentRotation.y, angle), rotateSpeed * frameDelay);
            //currentRotation.x = 0;
            //arm.transform.rotation = currentRotation;
            currnetFrameDelay = 0;
        }

        ReLoad();

    }
    void ReLoad()
    {
        if (shootDelay > shootDelayTime + 0.2f)
        {
            Shoot();
        }
        else if(shootDelay >= shootDelayTime - 0.5f)
        {
            arm.GetComponent<Animator>().Play("Unit_Shotgun_Reload", 0, (0.5f - (shootDelayTime - shootDelay)) / 0.5f);
        }

        // 20230723 ÅºÇÇ ¹èÃâÀº ½Ã°£ ³²À¸¸é ÁøÇà
        //GameObject Shell = Instantiate(shell, transform.position, Quaternion.identity);

        //// RandomPower
        //Random.Range(0, shootDelay);
        //Vector2 shellPower = new Vector2(Random.Range(minShellPower.x, maxShellPower.x) * transform.right.x, Random.Range(minShellPower.y, maxShellPower.y));
        //Shell.GetComponent<Rigidbody2D>().AddForce(shellPower, ForceMode2D.Impulse);
    }

    void Shoot()
    {
        Vector2 pos = shootPoint;

        unitStateManager.bulletManager.PlayShootEffect(arm.transform ,pos, arm.transform.rotation); // ÃÑ ¹ß»ç ½ºÆÄÅ©
        ShotgunPallet(pos); // ÃÑ¾Ë ¹ß»ç
        shootDelay = -0.1f;
        Aim_Enter();
    }


    // °¢µµ ¸¶´Ù ·£´ýÇÏÁö¸¸ ÃÖ¼ÒÇÑÀ¸·Î ÀÏÁ¤ÇÏ°Ô »ê°³µÇÁö¸¸ ±â´É
    void ShotgunPallet(Vector2 _pos)
    {
        float angleInterval = spreadAngle / shotgunPallet; // °£°Ý
        Vector3 baseangle = arm.transform.rotation.eulerAngles;
        float startAngle = baseangle.z;
        startAngle += -spreadAngle / 2;

        for (int i = 0; shotgunPallet  > i;i++ )
        {
            // °¢µµ ÁöÁ¤
            float currentAngle = startAngle;
            currentAngle += angleInterval * i;


            // ·£´ý ÀÎµ¦½º
            float randomIndex = Random.Range(-2f, 2f);
            currentAngle += randomIndex;

            // ÃÑ¾Ë »ý¼º
            unitStateManager.bulletManager.PlayShootBullet(arm.transform, _pos, Quaternion.Euler(0, 0, currentAngle));
        }

    }



    void Aim_Exit()
    {
        //unitStateManager.objectAnimClass.ChangeAnimstr("Unit_Shotgun_Aim");
        arm.SetActive(false);
    }

}
