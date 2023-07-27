using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    ParticleSystem particlesystem; // ����ũ
    SpriteRenderer spriterenderer_Bullet;
    SpriteRenderer spriterenderer_Mask;

    public float speed = 20;
    public float maxLenth = 0.5f; // ���� �ִ�ġ
    public float minLenth = 0.2f; // ���� �ּ�ġ

    private float timeActivated = 0;
    public  float timeDestroy = 2;

    bool isReflect = false;
    bool isHitWall = false;

    private void Awake()
    {
        particlesystem = GetComponentInChildren<ParticleSystem>(); // ����ũ
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
            if (!isHitWall) // �Ѿ��� ���ο쿡���� �ε巯�� �������� �־�� �ϱ⿡ FixedUpdate�� �ƴ� Update
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
            float angleInRadians = transform.rotation.z * Mathf.Deg2Rad; // ������ �������� ��ȯ�մϴ�.
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
