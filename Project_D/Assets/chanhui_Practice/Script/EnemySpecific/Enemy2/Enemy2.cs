using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Entity
{
    public E2_IdleState idleState { get; private set; }
    public E2_MoveState moveState { get; private set; }
    public E2_PlayerDetectedState playerDetectedState { get; private set; }
    //public E2_ChargeState chargeState { get; private set; }
    public E2_LookForPlayerState lookForPlayerState { get; private set; }
    public E2_MeleeAttackState meleeAttackState { get; private set; }
    public E2_StunState stunState { get; private set; }
    public E2_DeadState deadState { get; private set; }

    [SerializeField]
    private S_IdleState idleStateData;
    [SerializeField]
    private S_MoveState moveStateData;
    [SerializeField]
    private S_PlayerDetected playerSetectedData;
    [SerializeField]
    private S_MeleeAttack meleeAttackStateData;
    [SerializeField]
    private S_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private S_StunState stunStateData;
    [SerializeField]
    private S_DeadState deadStateData;
    /*
    [SerializeField]
    private S_ChargeState chargeStateData;
    
    
    
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
        lookForPlayerState = new E2_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new E2_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new E2_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(moveState);
    }

    public override void Damage(AttackDetails attackDetails)
    {
        base.Damage(attackDetails);

        if(isDead)
        {
            stateMachine.ChangeState(deadState);
        }
        else if(isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
        else if(!CheckPlayerInMinAgroRange())
        {
            lookForPlayerState.SetTurnImmediately(true);
            stateMachine.ChangeState(lookForPlayerState);
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }
}
