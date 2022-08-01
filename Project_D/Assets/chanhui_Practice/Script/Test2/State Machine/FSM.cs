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

    // ���� �ִ� ���¸� �����ϴ� �Լ�
    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }



}
