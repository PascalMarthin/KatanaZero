using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    ParticleSystem particlesystem; // 스파크
    SpriteRenderer spriterenderer_Bullet;
    SpriteRenderer spriterenderer_Mask;

    public float speed = 20;
    public float maxLenth = 0.5f; // 길이 최대치
    public float minLenth = 0.2f; // 길이 최소치

    private float timeActivated = 0;
    public  float timeDestroy = 2;

    bool isReflect = false;
    bool isHitWall = false;

    private void Awake()
    {
        particlesystem = GetComponentInChildren<ParticleSystem>(); // 스파크
        spriterenderer_Bullet = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriterenderer_Mask = transform.GetChild(1).GetComponent<SpriteRenderer>();
        //particlesystem.transform.localRotation = Quaternion.Euler(0,0,180);
    }

    private void OnEnable()
    {
        SetTurnOn();
        isReflect = false;
        isHitWall = false;
        timeActivated = 0;
        SetTransformScale(new Vector3(minLenth, maxLenth, 1));
    }
    public void Setreflect(Transform _playerTransform)
    {
        isReflect = true;
    }

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            if (!isHitWall) // 총알은 슬로우에서도 부드러운 움직임이 있어야 하기에 FixedUpdate가 아닌 Update
            {
                transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
                Vector3 localScale = Vector3.Lerp(new Vector3(minLenth, maxLenth, 1), new Vector3(maxLenth * Time.timeScale, 0.5f, 1), Mathf.Min(timeActivated * 10f, 1f));
                SetTransformScale(localScale);

            }
        }
    }

    void SetTransformScale(Vector3 _scale)
    {
        spriterenderer_Bullet.transform.localScale = _scale;
        spriterenderer_Mask.transform.localScale = _scale;
    }

    private void LateUpdate()
    {
        if(Time.timeScale > 0)
        {
            timeActivated += PlayManager.gameDeltaTime;
            if (timeActivated > timeDestroy || (isHitWall && particlesystem.isStopped))
            {
                gameObject.SetActive(false);
            }
        }

    }

    void SetTurnOn()
    {
        spriterenderer_Bullet.transform.localScale = Vector3.one;
        //spriterenderer_Mask.enabled = true;
    }
    void SetTurnOff()
    {
        spriterenderer_Bullet.transform.localScale = Vector3.zero;
        //spriterenderer_Mask.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string str = collision.tag;
        if ((str == "UNIT" && isReflect) || (str == "PLAYER" && !isReflect))
        {
            //Kill
            float angleInRadians = transform.rotation.z * Mathf.Deg2Rad; // 각도를 라디안으로 변환합니다.
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            direction.y += 0.5f;
            direction.x *= transform.right.x;
            collision.gameObject.GetComponent<ObjectStateManager>().UnitDeath(transform, direction, speed / 2);
        }
        else 
        {
            HashSet<string> tagsToCompare = new HashSet<string>() { "WALL", "GROUND", "OBJECT" };
            if (tagsToCompare.Contains(str))
            {
                // Destory
                isHitWall = true;
                SetTurnOff();
                particlesystem.Play();
            }
        }
    }

    private void OnDisable()
    {
        SetTurnOn();
    }
}
