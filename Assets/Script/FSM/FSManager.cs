using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    public delegate void Function();
    public delegate void FunctionAddRunningtime(StateInfo _Info);

public class FSManager : MonoBehaviour
{
    public FSMState idleState;

    public FSMState currentState { get; private set; }
    private FSMState postState;
    private StateInfo stateInfo;

    private Dictionary<string, FSMState> dictionaryState = new Dictionary<string, FSMState>();

    // StateChangeFunction
    public Function StateChangeUpdate;

    //void Start()
    //{
    //    //ResetState();
    //}

    void FixedUpdate()
    {
        if (currentState.FixedUpdate != null)
        {
            currentState.FixedUpdate(stateInfo);
        }
    }

    void Update()
    {
        currentState.Update(stateInfo);
        stateInfo.runningTime += PlayManager.gameDeltaTime;

        if (StateChangeUpdate != null)
        {
            StateChangeUpdate();
        }
    }

    void LateUpdate()
    {
        if(currentState.LateUpdate != null)
        {
            currentState.LateUpdate(stateInfo);                             
        }
    }


    public FSMState CreateState(string _statestring, (Function, FunctionAddRunningtime, FunctionAddRunningtime, FunctionAddRunningtime, FunctionAddRunningtime) _fTuple)
    {
        FSMState newState = new FSMState(_statestring, _fTuple);
        dictionaryState.Add(_statestring, newState);
        return newState;
    }

    public void ResetState()
    {
        currentState = idleState;
        currentState.Enter();
        stateInfo.runningTime = 0;
    }
    public void ResetState(string _statestring)
    {
        FSMState newState = FindState(_statestring);
        if (newState != null)
        {
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.Log("We Cant Find State what name is " + _statestring);
        }
        stateInfo.runningTime = 0;
    }

    public FSMState FindState(string _statestring)
    {
        FSMState findState = null;
        dictionaryState.TryGetValue(_statestring, out findState);
        return findState;
    }

    public void ChangeState(string _statestring)
    {
        FSMState newState = FindState(_statestring);
        if(newState != null)
        {
            if(newState == currentState)
            {
                Debug.Log("Change same State --> " + _statestring);
                return;
            } 
            postState = currentState;
            if (postState != null)
            {
                stateInfo.PostStatename = postState.name;
            }
            currentState.Exit(stateInfo);
            currentState = newState;
            currentState.Enter();

            stateInfo.runningTime = 0;
        }
        else
        {
            Debug.Log("We Cant Find State what name is " + _statestring);
        }
    }

    public class FSMState
    {
        public string name { get; private set; }


        public FSMState(string _name, (Function , FunctionAddRunningtime, FunctionAddRunningtime, FunctionAddRunningtime, FunctionAddRunningtime) _fTuple)
        {
            name = _name;
            Enter = _fTuple.Item1;
            FixedUpdate = _fTuple.Item2;
            Update = _fTuple.Item3;
            LateUpdate = _fTuple.Item4;
            Exit = _fTuple.Item5;
        }



        // 상태가 시작 할 때
        public Function Enter
        { get; private set; }

        // 상태 FixedUpdate
        public FunctionAddRunningtime FixedUpdate
        { get; private set; }

        // 상태 Update
        public FunctionAddRunningtime Update
        { get; private set; }

        // 상태 LateUpdate
        public FunctionAddRunningtime LateUpdate
        { get; private set; }

        // 상태에서 나갈 때
        public FunctionAddRunningtime Exit
        { get; private set; }

    }
}
public struct StateInfo
{
    public float runningTime;
    public string PostStatename;
}