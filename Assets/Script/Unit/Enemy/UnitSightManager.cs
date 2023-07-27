using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSightManager : MonoBehaviour
{
    // Manager
    UnitSearchManager searchManager;

    // Componenet
    private BoxCollider2D Collider2D;

    // Ray
    private float findDistance;
    private LayerMask guardMask;
    public float rayPivot = 0.1f;

    void Start()
    {
        searchManager = transform.parent.gameObject.GetComponent<UnitSearchManager>();
        Collider2D = GetComponent<BoxCollider2D>();

        findDistance = Collider2D.size.x;
        // ���� ��󿡼� ���� ����
        {
            guardMask = PlayManager.unitLayer;
            guardMask += PlayManager.searchLayer;
            guardMask = ~guardMask;
        }
    }

    public bool IsSightIn(Vector2 _Pos)
    {
        Vector2 playerDir = _Pos - new Vector2(transform.position.x , transform.position.y);
        Vector2 rayPos = transform.position;
        rayPos.x -= (transform.right.x * rayPivot);
        RaycastHit2D raycastInfo = Physics2D.Raycast(rayPos, playerDir, findDistance, guardMask);
        if(raycastInfo.collider.gameObject.CompareTag("PLAYER"))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (gObject.CompareTag("PLAYER"))
        {
            // ���װ� �þ߾ȿ� �ִ°�?
            if (!gObject.GetComponent<ObjectStateManager>().isDeath && IsSightIn(gObject.transform.position))
            {
                searchManager.VisiblePlayer(gObject.transform.position);
            }
            else
            {
                searchManager.UnvisiblePlayer();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject gObject = collision.gameObject;
        if (gObject.CompareTag("PLAYER"))
        {
            searchManager.UnvisiblePlayer();
        }
    }
}
