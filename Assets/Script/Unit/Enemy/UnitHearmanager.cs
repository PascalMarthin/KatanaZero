using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �þ߰� �Ⱥ��̴� �ݴ� ���⿡�� �þ�ó�� Ȱ�����ִ� Ŭ����
public class UnitHearmanager : MonoBehaviour
{
    // Manager
    UnitSearchManager searchManager;

    // Component
    private CircleCollider2D circleCollider;

    void Start()
    {
        searchManager = transform.parent.gameObject.GetComponent<UnitSearchManager>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (gObject.CompareTag("PLAYER"))
        {
            if(!gObject.GetComponent<ObjectStateManager>().isDeath)
            {
                searchManager.HearPlayer(gObject.transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        searchManager.UnHearPlayer();
    }


}
