using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    // ¹Ù´Ú ·¹ÀÌ¾î
    public static LayerMask groundLayer = 0;
    static public float gameDeltaTime { get; private set; } = 0;
    static public float gameFixedTime { get; private set; } = 0;
    static float timeDeltaScale = 1;
    static float timeFixedScale = 1;

    // Cursor
    public Texture2D cursorTexture;

    private void Awake()
    {
        groundLayer = LayerMask.NameToLayer("GROUND");
    }
    void Start()
    {
        gameDeltaTime = Time.deltaTime;
        gameFixedTime = Time.fixedDeltaTime;

        
        Cursor.SetCursor(cursorTexture, cursorTexture.Size() * 0.5f, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        gameDeltaTime = Time.deltaTime * timeDeltaScale;
    }

    public void SetDeltatimeScale(float _scale)
    {
        timeDeltaScale = _scale;
    }

    public void SetFixedtimeScale(float _scale)
    {
        timeFixedScale = _scale;
        gameFixedTime *= timeFixedScale;
    }
}
