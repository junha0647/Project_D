using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Entity
{
    public E2_IdleState idleState { get; private set; }
    public E2_MoveState moveState { get; private set; }
    public E2_PlayerDetectedState playerDetectedState { get; private set; }
    //public E2_ChargeState chargeState { get; private set; }
    //public E2_LookForPlayerState lookForPlayerState { get; private set; }
    public E2_MeleeAttackState meleeAttackState { get; private set; }
    //public E2_StunState stunState { get; private set; }
    //public E2_DeadState deadState { get; private set; }

    [SerializeField]
    private S_IdleState idleStateData;
    [SerializeField]
    private S_MoveState moveStateData;
    [SerializeField]
    private S_PlayerDetected playerSetectedData;
    [SerializeField]
    private S_MeleeAttack meleeAttackStateData;

    /*
    [SerializeField]
    private S_ChargeState chargeStateData;
    [SerializeField]
    private S_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private S_StunState stunStateData;
    [SerializeField]
    private S_DeadState deadStateData;
    */

    [SerializeField]
    private Transform meleeAttackPosition;

    public override void Start()
    {
        base.Start();

        moveState = new E2_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new E2_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new E2_PlayerDetectedState(this, stateMachine, "charge", playerSetectedData, this);
        meleeAttackState = new E2_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);

        stateMachine.Initialize(moveState);
    }

    public override void Damage(AttackDetails attackDetails)
    {
        base.Damage(attackDetails);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
