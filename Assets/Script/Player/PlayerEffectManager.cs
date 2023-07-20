using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    // Effect
    public GameObject gameObjectsEffect_Land;
    public GameObject gameObjectsEffect_Jump;

    // Run Effect
    public GameObject gameObjectsEffect_Run;
    public int maxSizeEffect_Run = 4;
    private Queue<ParticleSystem> queueSleepEffect_Run = new Queue<ParticleSystem>();
    private LinkedList<ParticleSystem> listActiveEffect_Run = new LinkedList<ParticleSystem>();
    public Vector3 run_EffectOffset = new Vector3(-0.004f, -0.826687f, 0);


    public Vector2 posOffSet_Land = Vector2.zero;
    public Vector2 posOffSet_Jump = Vector2.zero;

    void Start()
    {
        for (int i = 0; i < maxSizeEffect_Run; i++)
        {
            GameObject gObject = Instantiate(gameObjectsEffect_Run);
            queueSleepEffect_Run.Enqueue(gObject.GetComponent<ParticleSystem>());
        }
    }

    void Update()
    {
        CheckCloudEffect();
    }

    void CheckCloudEffect()
    {
        // 활성화 목록
        if (listActiveEffect_Run.Count > 0)
        {
            List<ParticleSystem> listgarbage = new List<ParticleSystem>();
            foreach (var ps in listActiveEffect_Run)
            {
                if (ps.isStopped)
                {
                    queueSleepEffect_Run.Enqueue(ps);
                    listgarbage.Add(ps);
                }
            }

            foreach (var ps in listgarbage)
            {
                listActiveEffect_Run.Remove(ps);
            }
        }
    }

    public void PlayLandCloudEffect(Vector2 _pos)
    {
        Instantiate(gameObjectsEffect_Land, _pos + posOffSet_Land, Quaternion.identity);
    }

    public void PlayJumpCloudEffect(Vector2 _pos)
    {
        Instantiate(gameObjectsEffect_Jump, _pos + posOffSet_Jump, Quaternion.identity);
    }

    public ParticleSystem PlayRunCloudEffect()
    {
        // 할당
        ParticleSystem ps = null;
        if (queueSleepEffect_Run.Count > 0) 
        {
            ps = queueSleepEffect_Run.Dequeue();
        }
        else  // 비어있는 파티클 시스템이 없는 경우
        {
            ps = listActiveEffect_Run.First.Value;
            ps.Stop();
            listActiveEffect_Run.Remove(listActiveEffect_Run.First);
        }

        // 위치 조정
        float dir = Input.GetAxisRaw("Horizontal");
        if (dir < 0)
        {
            Quaternion currentQ = ps.transform.rotation;
            ps.transform.rotation = new Quaternion(currentQ.x, 180, currentQ.z, currentQ.w);
        }
        else
        {
            Quaternion currentQ = ps.transform.rotation;
            ps.transform.rotation = new Quaternion(currentQ.x, 0, currentQ.z, currentQ.w);
        }
        Vector3 offset = run_EffectOffset;
        offset.x *= dir;

        ps.transform.position = transform.position + offset;
        ps.Play();

        listActiveEffect_Run.AddLast(ps);
        return ps;
    }

    public void StopRunCloudEffect(ParticleSystem _ps)
    {
        _ps.Stop();
    }
}
