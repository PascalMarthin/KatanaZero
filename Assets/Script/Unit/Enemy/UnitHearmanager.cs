using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시야가 안보이는 반대 방향에도 시야처럼 활용해주는 클래스
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
