using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public FSM stateMachine;

    public S_Entity entityData;

    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject aliveGo { get; private set; }
    public AnimationToStatemachine atsm { get; private set; }
    public int lastDamageDirection { get; private set; }

    [SerializeField]
    private Transform PlayerTr;
    [SerializeField]
    private Transform EnemyTr;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private Transform groundCheck;


    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;
    

    private Vector2 velocityWorkspace;

    protected bool isStunned;
    protected bool isDead;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        

        aliveGo = transform.Find("Alive").gameObject;
        rb = aliveGo.GetComponent<Rigidbody2D>();
        anim = aliveGo.GetComponent<Animator>();
        atsm = aliveGo.GetComponent<AnimationToStatemachine>();

        stateMachine = new FSM();
    }

    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();

        if(Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    // ���Ϳ� �ӵ��� ���� �� �ִ� �Լ�
    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void SetVel(float speed)
    {
        velocityWorkspace = new Vector2(PlayerTr.position.x, EnemyTr.position.y);
        EnemyTr.position = Vector2.MoveTowards(EnemyTr.position, velocityWorkspace, Time.deltaTime * speed);
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = velocityWorkspace;
    }

    // ���� üũ�ϴ� �Լ�
    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGo.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    // �ٴ��� üũ�ϴ� �Լ�
    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, entityData.groundCheckRadius, entityData.whatIsGround);
    }

    // �÷��̾� �ν� �ּ� ����
    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGo.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }
    // �÷��̾� �ν� �ִ� ����
    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGo.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }
    // �ν��� �÷��̾ �������� �� �ִ� ���� 
    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGo.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(rb.velocity.x, velocity);
        rb.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    // ������ ���� �Լ�
    public virtual void Damage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time;

        currentHealth -= attackDetails.damageAmount;
        currentStunResistance -= attackDetails.stunDamageAmount;

        DamageHop(entityData.damageHopSpeed);

        Debug.Log("����");
        //Instantiate(entityData.hitParticle, aliveGo.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        if(attackDetails.position.x > aliveGo.transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }

        if(currentStunResistance <= 0)
        {
            isStunned = true;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }

    // ���� ��ȯ�ϴ� �Լ�
    public virtual void Flip()
    {
        facingDirection *= -1;
        aliveGo.transform.Rotate(0f, 180f, 0f);
    }

    // ��,�ٴڿ� Raycast�� ���̰� ���ִ� �Լ�
    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.2f);
    }

}
