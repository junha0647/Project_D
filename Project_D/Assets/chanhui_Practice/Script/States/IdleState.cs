using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected S_IdleState stateData;
    [Tooltip("Flip 회전")]
    protected bool flipAfterIdle;
    [Tooltip("다른 상태가 되었는지 아닌지 유휴 상태 시간")]
    protected bool isIdleTimeOver;
    protected bool isPlayerInMinAgroRange;

    [Tooltip("Idle 시간 주기")]
    protected float idleTime;

    public IdleState(Entity entity, FSM stateMachine, string animBoolName, S_IdleState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        entity.SetVelocity(0f);
        isIdleTimeOver = false;
        
        SetRandomIdleTime();
    }

    public override void Exit()
    {
        base.Exit();

        if(flipAfterIdle)
        {
            entity.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // idle 시간이 끝나면 true
        if(Time.time >= startTime + idleTime)
        {
            isIdleTimeOver = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void SetFlipAfterIdle(bool flip)
    {
        flipAfterIdle = flip;
    }

    private void SetRandomIdleTime()
    {
        idleTime = Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
    }
}
