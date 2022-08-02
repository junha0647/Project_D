using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FSM stateMachine;
    protected Entity entity;

    protected float startTime;

    protected string animBoolName;

    public State(Entity entity, FSM stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;    
    }

    // 새로운 상태를 넣어주고, 상태가 시작하는 시간을 저장하는 함수.
    public virtual void Enter()
    {
        startTime = Time.time;
        entity.anim.SetBool(animBoolName, true);
        DoChecks();
    }

    // 상태를 끝내는 함수.
    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
    }


    public virtual void LogicUpdate()
    {

    }
    

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }


}
