using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("���ݷ�")][SerializeField] private float OffensePower;
    [Header("����")][SerializeField] private float DefensePower;
    [Header("ü��")][SerializeField] private int HealthPoint;
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
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        Move();
        Jump();

        Attack();
    }

    void Move()
    {
        // ����Ű �Է� //
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // �̵��ӵ� ���� //
        if (rigid.velocity.x > Speed)
        {
            rigid.velocity = new Vector2(Speed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < Speed * (-1))
        {
            rigid.velocity = new Vector2(Speed * (-1), rigid.velocity.y);
        }

        // �̲��� ���� //
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.01f, rigid.velocity.y);
        }

        // �¿� ���� //
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // �뽬 //
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(StaminaPoint > 50)
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

    void Jump()
    {
        // ���� //
        if (!Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            if(!isJump)
            {
                rigid.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            }
        }

        // ���� �� jumpNum = 0 �ʱ�ȭ //


        // ���� ����
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("���� ����");
            // ���� ���� �ӵ��� ������(JumpForce)���� ������� �ʰ� �߷�(Gravity)���� ����Ѵ�.
        }
    }

    private float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;
    void Attack()
    {
        if(curTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach(Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.tag);
                    if(collider.tag == "Enemy")
                    {
                        collider.GetComponent<EnemyController>().TakeDamage(1);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "doubleJumpItem")
        {
            Debug.Log("���� ���� ��������");
            canDoubleJump = true;
            Destroy(collision.gameObject);
        }
    }
}
