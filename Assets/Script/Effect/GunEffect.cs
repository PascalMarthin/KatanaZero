using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect : MonoBehaviour
{
    public GameObject GunfireEffect;
    public GameObject GunsmokeEffect;

    private void OnEnable()
    {
        Play();
    }
    public void Play()
    {
        // ���� ����Ʈ ���
        int gunFireIndex = Random.Range(0, 3);
        GunfireEffect.GetComponent<Animator>().SetInteger("RandomIndex", gunFireIndex);
        int gunsmokeIndex = Random.Range(0, 3);
        GunsmokeEffect.GetComponent<Animator>().SetInteger("RandomIndex", gunsmokeIndex);
    }

    private void LateUpdate()
    {
        if(Time.timeScale > 0)
        {
            if (!(GunfireEffect.activeSelf || GunsmokeEffect.activeSelf)) // �Ѵ� off��
            {
                gameObject.SetActive(false);
                GunfireEffect.SetActive(true);
                GunsmokeEffect.SetActive(true);
            }
        }
    }
}
