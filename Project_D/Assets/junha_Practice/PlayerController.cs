using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("���ݷ�")][SerializeField] private float OffensePower;
    [Header("����")][SerializeField] private float DefensePower;
    [Header("ü��")][SerializeField] private float HealthPoint;
    [Header("���")][SerializeField] private int StaminaPoint;
    [Header("�̵��ӵ�")][SerializeField] private int Speed;
    [Header("������")][SerializeField] private int JumpForce;
    [Header("��ðŸ�")][SerializeField] private int DashForce;
    [Header("ȸ�ǽð�")][SerializeField] private int EvasionTime;

    [Header("�˹�Ÿ�")][SerializeField] private int KnockBack_distance;
    [Header("�ǰ� �� �����ð�")][SerializeField] private int InvincibleTime;


    private bool isJump = false;
    private int jumpNum = 0;
    private bool canDoubleJump = false;

    private bool isDash = false;
    private float dashTime = 0;
    private KeyCode lastKeyCode;
    private bool canDash = false;

    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    Animator animator;

    // ���� �ǵ��� �κ�
    private AttackDetails attackDetails;
    [SerializeField] private float stunDamageAmount = 1f;

    //--------------------------------
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();

        Attack();
    }

    // === �⺻ �̵� ���� : ����Ű �Է� === //
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // === �̵��ӵ� ���� === //
        if (rigid.velocity.x > Speed)
        {
            rigid.velocity = new Vector2(Speed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < Speed * (-1))
        {
            rigid.velocity = new Vector2(Speed * (-1), rigid.velocity.y);
        }

        // === �̵� �� �̲��� ���� === //
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.01f, rigid.velocity.y);
        }

        // === �¿� ���� === //
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // === �ȱ� �ִϸ��̼� === // 
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("isRun", false);
        }
        else
        {
            animator.SetBool("isRun", true);
        }

        // === �뽬 === //
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (StaminaPoint > 50)
            {
                Debug.Log("�뽬");
                StaminaPoint -= 50;
            }
            else
            {
                Debug.Log("����� �����մϴ�.");
            }
        }
    }

    // === ���� === //
    void Jump()
    {
        if (!Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            if (!isJump)
            {
                animator.SetBool("isJump", true);

                rigid.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            }
        }

        // ���� �� jumpNum = 0 �ʱ�ȭ //

        if (rigid.velocity.y <= 0)
        {
            Vector2 frontVec = new Vector2(rigid.position.x + rigid.velocity.x * 0.025f, rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 1f)
                {
                    animator.SetBool("isJump", false);
                }
            }
            // ���⼭ ������ ���� rayHit�� 2f�� distance�� ����(1.2f)�� �� �����ؾ� ��.
        }


        // === ���� ���� === //
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("���� ����");
            // ���� ���� �ӵ��� ������(JumpForce)���� ������� �ʰ� �߷�(Gravity)���� ����Ѵ�.
        }
    }

    float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;
    
    // ���� �ǵ帲
    public float attackDamage = 5;
    void Attack()
    {
        if(curTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                animator.SetTrigger("atk1");

                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);

                attackDetails.position = pos.position;
                attackDetails.damageAmount = attackDamage;
                attackDetails.stunDamageAmount = stunDamageAmount;

                foreach (Collider2D collider in collider2Ds)
                {
                    collider.transform.parent.SendMessage("Damage", attackDetails);
                    Debug.Log(collider.tag);
                    if (collider.tag == "Enemy")
                    {
                        // ���� ���� ������ ���� (���� ���� - (���ݷ�(Offense Power) * 1))
                        // collider.GetComponentInChildren<Enemy1>().Damage(attackDetails);
                        // collider.GetComponent<EnemyController>().TakeDamage(OffensePower);
                        // �ش� ������Ʈ ��ũ��Ʈ���� �Լ� ���ڿ��� def ���ִ� �ɷ� �ذ�.
                    }
                }

                Debug.Log("����");
                curTime = coolTime;
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }
    }
    
    // === �÷��̾� ���� ���� ����� === //
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    // === �÷��̾�� �ٸ� ������Ʈ ��ȣ�ۿ� === //
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "doubleJumpItem")
        {
            Debug.Log("���� ���� ��������");
            canDoubleJump = true;
            Destroy(collision.gameObject);
        }
    }

    private void Damage(AttackDetails details)
    {
        HealthPoint -= details.damageAmount;
    }
}