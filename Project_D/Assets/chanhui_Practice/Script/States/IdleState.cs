using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected S_IdleState stateData;
    [Tooltip("Flip ȸ��")]
    protected bool flipAfterIdle;
    [Tooltip("�ٸ� ���°� �Ǿ����� �ƴ��� ���� ���� �ð�")]
    protected bool isIdleTimeOver;
    protected bool isPlayerInMinAgroRange;

    [Tooltip("Idle �ð� �ֱ�")]
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

        // idle �ð��� ������ true
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
