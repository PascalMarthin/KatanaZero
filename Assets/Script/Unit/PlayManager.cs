using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using static PlayManager.SceneData;
using System;
using System.Linq;

public class PlayManager : MonoBehaviour
{
    // 레이어
    public static LayerMask wallLayer { get; private set; } = 0;
    public static LayerMask groundLayer { get; private set; } = 0;
    public static LayerMask unitLayer { get; private set; } = 0;
    public static LayerMask objectLayer { get; private set; } = 0;
    public static LayerMask playerLayer { get; private set; } = 0;
    public static LayerMask searchLayer { get; private set; } = 0;


    // Time
    static public float gameDeltaTime { get; private set; } = 0;
    static public float gameFixedTime { get; private set; } = 0;
    static float timeDeltaScale = 1;
    static float timeFixedScale = 1;

    /// <summary>
    /// Time Offset
    /// </summary>
    /// 
    [Header("KillSlow Info")]
    // KILLSLOW
    public float timeKillSlow = 0.5f; // 0 ~ 1;
    public float timeHoldingKillSlow = 0.2f; // 0 ~ 1;

    [Header("TimeSlow Info")]
    // TimeSlow
    public KeyCode keySlow = KeyCode.LeftShift;
    public float minTimeSlow = 0.3f; // 0~ 1
    public float timeUntillminSlow = 0.5f; // 최소 시간까지 돌입하는데 걸리는 시간 (RealTime)
    private bool isSlow = false;
    private float currentTimeSclae = 1;

    [SerializeField]
    private float inSlowMode;

    [Header("TimeScale")]
    [SerializeField]
    private float readOnlyTimeScale = 0;


    // PlayerState
    private bool isGameOver = false;

    [Header("Cursor")]
    // Cursor
    public Texture2D cursorTexture;

    [Header("SaveInfo")]
    // Save
    public float extension = 0.2f;
    //public int framePerSaveScene = 3;         // FixedUpdate 로 찍을 시절 남은 더미 데이터
    //private int currnetFramePerSaveScene = 0; // FixedUpdate 로 찍을 시절 남은 더미 데이터

    public float timePerSaveScene = 0.02f;
    private float currnettimePerSaveScene = 0;

    public Stack<SceneData> sceneDatas = new Stack<SceneData>(); // 씬 데이터
    SceneData beforeSceneData; // 이전 씬 데이터 (메소드 정적 변수 사용 불가로 이곳으로 이동)

    // Rewind
    private bool isStartRewind = false;


    // 멀티톤
    public static PlayManager Inst
    {
        get;
        private set;
    }

    private void Awake()
    {
        groundLayer = 1 << LayerMask.NameToLayer("GROUND");
        unitLayer = 1 << LayerMask.NameToLayer("UNIT");
        objectLayer = 1 << LayerMask.NameToLayer("OBJECT");
        playerLayer = 1 << LayerMask.NameToLayer("PLAYER");
        searchLayer = 1 << LayerMask.NameToLayer("SEARCH");
        wallLayer = 1 << LayerMask.NameToLayer("WALL");



        Application.targetFrameRate = 60; // 프레임 제한

        Cursor.SetCursor(cursorTexture, cursorTexture.Size() * 0.5f, CursorMode.Auto);
    }
    void Start()
    {
        gameDeltaTime = Time.deltaTime;
        gameFixedTime = Time.fixedDeltaTime;
        currnettimePerSaveScene = timePerSaveScene;
    }

    void SaveScene()
    {
        SceneData data = new SceneData();
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            // 화면 내 GameObject 검출
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(obj.transform.position);
            if (viewportPos.x >= -extension && 
                viewportPos.x <= 1 + extension && 
                viewportPos.y >= -extension && 
                viewportPos.y <= 1 + extension && 
                viewportPos.z > 0 &&
                viewportPos.z < 20)// BackGround 제외
            {
                if(obj.GetComponent<SpriteRenderer>())
                {
                    data.AddObject(obj);
                }
            }
        }
        sceneDatas.Push(data);
    }

    public class SceneData
    {
        public Dictionary<int ,ObjectData> objectData { get; private set; } = new Dictionary<int, ObjectData>(); // 유일성이 보장됨, 검색 및 삭제

        public void AddObject(GameObject _gameObject)
        {
            int id = _gameObject.GetInstanceID();
            objectData.Add(id, new ObjectData(_gameObject));
        }

        // 여기 해시셋 내 오브젝트 데이터들을 인자로 온 씬데이터의 오브젝트들을 
        public void ExceptWithData(SceneData _sceneData)
        {
            // LINQ;
            var keysToRemove = objectData.Keys.Intersect(_sceneData.objectData.Keys).ToList();
            foreach (var key in keysToRemove)
            {
                objectData.Remove(key);
            }
        }
                                                                                                                                
        public class ObjectData
        {
            public GameObject gameObject { get; private set; }
            // transform
            public Vector3 worldPos { get; private set; }
            public Quaternion worldRot { get; private set; }
            public Vector3 localScale { get; private set; }
            public Sprite sprite { get; private set; }

            public ObjectData(GameObject _gameObject)
            {
                gameObject = _gameObject;
                sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
                worldPos = gameObject.transform.position;
                worldRot = gameObject.transform.rotation;
                localScale = gameObject.transform.localScale;
            }
        }
    }


    void Update()
    {
        gameDeltaTime = Time.deltaTime * timeDeltaScale;
        if (!isStartRewind)
        {
            TimeScale();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            StartRewind();
        }
    }

    void TimeScale()
    {
        readOnlyTimeScale = Time.timeScale;
        if (!isGameOver)
        {
            if (Input.GetKeyDown(keySlow))
            {
                isSlow = true;
                // 반응성을 기대해야하기에 코루틴 x
            }
            else if (Input.GetKey(keySlow))
            {
                // 게이지 닳는 코드
            }
            else if (Input.GetKeyUp(keySlow))
            {
                isSlow = false;
            }
        }
        else
        {
            isSlow = false;
        }


        // 슬로우 모드 진입
        if (isSlow)
        {
            inSlowMode += Time.unscaledDeltaTime;
            if (inSlowMode > timeUntillminSlow)
            {
                inSlowMode = timeUntillminSlow;
            }

            float LerpTime = 1 - ((timeUntillminSlow - inSlowMode) / timeUntillminSlow);
            currentTimeSclae = Mathf.Lerp(1, minTimeSlow, LerpTime);
        }
        else
        {
            if (inSlowMode > 0)
            {
                inSlowMode -= Time.unscaledDeltaTime;
                if (inSlowMode < 0)
                {
                    inSlowMode = 0;
                }
                float LerpTime = 1 - ((timeUntillminSlow - inSlowMode) / timeUntillminSlow);
                currentTimeSclae = Mathf.Lerp(1, minTimeSlow, LerpTime);
            }
        }
        Time.timeScale = currentTimeSclae;
    }

    public void StartRewind()
    {
        StartCoroutine(CoroutineReWind());
    }
    IEnumerator CoroutineReWind()
    {
        beforeSceneData = null;
        isStartRewind = true;
        Time.timeScale = 0;
        while (sceneDatas.Count > 0)
        {
            SceneData data = sceneDatas.Pop();
            foreach (var pairData in data.objectData)
            {
                var SceneObjectData = pairData.Value;
                if (SceneObjectData.gameObject)
                {
                    GameObject gameObjectBefore = SceneObjectData.gameObject;

                    Component[] components = gameObjectBefore.GetComponents<Component>();
                    foreach (Component component in components)
                    {
                        MonoBehaviour monoBehaviour = component as MonoBehaviour;
                        if (monoBehaviour != null)
                        {
                            monoBehaviour.enabled = false;
                        }
                        else
                        {
                            Behaviour behaviour = component as Behaviour;
                            if (behaviour != null)
                            {
                                behaviour.enabled = false;
                            }
                            else
                            {
                                // Debug.Log("Cant Off this Component"); transform, rigidbody, spriterenderer
                            }
                        }
                    }

                    gameObjectBefore.transform.position = SceneObjectData.worldPos;
                    gameObjectBefore.transform.rotation = SceneObjectData.worldRot;
                    gameObjectBefore.transform.localScale = SceneObjectData.localScale;
                    SpriteRenderer spriteRenderer = gameObjectBefore.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = SceneObjectData.sprite;

                    gameObjectBefore.transform.parent.gameObject.SetActive(true);
                    gameObjectBefore.SetActive(true);
                }
            }

            {
                // 다음 프레임에서 존재한 Object지만 이전에는 존재하지 않는 것
                if (beforeSceneData != null)
                {
                    beforeSceneData.ExceptWithData(data);
                    foreach (var pairData in beforeSceneData.objectData)
                    {
                        var SceneObjectData = pairData.Value;
                        if (SceneObjectData.gameObject)
                        {
                            SceneObjectData.gameObject.SetActive(false);
                        }
                    }
                }
                beforeSceneData = data;
            }

            yield return null;
        }
        Debug.Log("End ReWind");
        Time.timeScale = 1;
        isStartRewind = false;

        SceneManager.LoadScene("SampleScene");

        yield break;
    }

    public void SetGameOver()
    {
        isGameOver = true;
    }

    IEnumerator CoroutinePlaySlow()
    {
        if(!isSlow)
        {
            Time.timeScale = Mathf.Min(currentTimeSclae, timeKillSlow);
            yield return new WaitForSecondsRealtime(timeUntillminSlow); // _time 동안 대기
            Time.timeScale = currentTimeSclae;
        }
        else
        {
            yield break;
        }
 
    }
    private void LateUpdate()
    {
        if (Time.timeScale > 0)
        {
            currnettimePerSaveScene += Time.deltaTime;
            if (currnettimePerSaveScene >= timePerSaveScene)
            {
                SaveScene(); // 저장
                currnettimePerSaveScene = 0;
            }
        }
    }



    // Time
    public void SetDeltatimeScale(float _scale)
    {
        timeDeltaScale = _scale;
    }

    public void SetFixedtimeScale(float _scale)
    {
        timeFixedScale = _scale;
        gameFixedTime *= timeFixedScale;
    }


    // SetTime
    public void PlayKillSlow()
    {
        PlayKillSlow(timeKillSlow, timeHoldingKillSlow);
    }
    public void PlayKillSlow(float _scale, float _time)
    {
        StartCoroutine(CoroutinePlayKillSlow(_scale, _time));
    }

    IEnumerator CoroutinePlayKillSlow(float _scale, float _time)
    {
        Time.timeScale = _scale;
        yield return new WaitForSecondsRealtime(_time); // _time 동안 대기
        Time.timeScale = 1;
    }





    private void OnEnable()
    {
        Inst = this;
    }

    private void OnDisable()
    {
        Inst = null;
    }
}
