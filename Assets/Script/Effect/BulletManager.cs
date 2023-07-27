using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BulletManager : MonoBehaviour
{
    public GameObject gunEffectObject;
    public GameObject bulletObject;

    public int minEffect = 5;


    private Queue<GameObject> queueSleepEffect = new Queue<GameObject>();
    private List<GameObject> gameEffectObjects = new List<GameObject>();

    private Queue<GameObject> queueSleepBullet = new Queue<GameObject>();
    private List<GameObject> gameBulletObjects = new List<GameObject>();


    private void Awake()
    {
        for(int i = 0; minEffect > i; i++)
        {
            CreateEffect();
        }

        for (int i = 0; minEffect * 5 > i; i++)
        {
            CreateBullet();
        }
    }

    GameObject CreateEffect()
    {
        GameObject gObject = Instantiate(gunEffectObject);
        queueSleepEffect.Enqueue(gObject);
        gObject.SetActive(false);
        gObject.transform.SetParent(transform); // 폴더 정리
        return gObject;
    }

    GameObject CreateBullet() 
    {
        GameObject gObject = Instantiate(bulletObject);
        queueSleepBullet.Enqueue(gObject);
        gObject.SetActive(false);
        gObject.transform.SetParent(transform); // 폴더 정리
        return gObject;
    }


    void Update()
    {
        CheckFreeEffect();
    }

    void CheckFreeEffect()
    {
        // 활성화 목록
        if (gameEffectObjects.Count > 0)
        {
            List<GameObject> listgarbage = new List<GameObject>();
            foreach (var effect in gameEffectObjects)
            {
                if (!effect.activeSelf)
                {
                    queueSleepEffect.Enqueue(effect);
                    listgarbage.Add(effect);
                }
            }

            foreach (var effect in listgarbage)
            {
                gameEffectObjects.Remove(effect);
            }
        }

        if (gameBulletObjects.Count > 0)
        {
            List<GameObject> listgarbage = new List<GameObject>();
            foreach (var bullet in gameBulletObjects)
            {
                if (!bullet.activeSelf)
                {
                    queueSleepBullet.Enqueue(bullet);
                    listgarbage.Add(bullet);
                }
            }

            foreach (var bullet in listgarbage)
            {
                gameBulletObjects.Remove(bullet);
            }
        }
    }

    public void PlayShootEffect(UnityEngine.Transform _transform, UnityEngine.Vector2 _localPos, UnityEngine.Quaternion _angle)
    {
        // 할당
        GameObject Efeect;
        if (queueSleepEffect.Count < 1)
        {
            CreateEffect();
        }
        Efeect = queueSleepEffect.Dequeue();

        Efeect.SetActive(true);
        Efeect.transform.position = UnityEngine.Vector2.zero;
        Efeect.transform.localPosition = _localPos;
        Efeect.transform.SetParent(_transform, false);
        Efeect.transform.rotation = _angle;
        Efeect.transform.SetParent(transform);
        gameEffectObjects.Add(Efeect);

    }

    public void PlayShootBullet(UnityEngine.Transform _transform, UnityEngine.Vector2 _startPos, UnityEngine.Quaternion _angle)
    {
        // 할당
        GameObject Bullet;
        if (queueSleepBullet.Count < 1)
        {
            CreateBullet();
        }
        Bullet = queueSleepBullet.Dequeue();

        Bullet.SetActive(true);
        Bullet.transform.position = UnityEngine.Vector2.zero;
        Bullet.transform.SetParent(_transform, false);
        Bullet.transform.localPosition = _startPos;
        Bullet.transform.localRotation = _angle;
        Bullet.transform.SetParent(transform);
        gameBulletObjects.Add(Bullet);
    }

}
