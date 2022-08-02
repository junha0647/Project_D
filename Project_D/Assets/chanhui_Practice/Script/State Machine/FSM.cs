using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public State currentState { get; private set; }

    public void Initialize(State startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    // 현재 있는 상태를 변경하는 함수
    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }



}
