using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("공격력")][SerializeField] private float OffensePower;
    [Header("방어력")][SerializeField] private float DefensePower;
    [Header("체력")][SerializeField] private int HealthPoint;
    [Header("기력")][SerializeField] private int StaminaPoint;
    [Header("이동속도")][SerializeField] private int Speed;
    [Header("점프력")][SerializeField] private int JumpForce;
    [Header("대시거리")][SerializeField] private int DashForce;
    [Header("회피시간")][SerializeField] private int EvasionTime;

    [Header("넉백거리")][SerializeField] private int KnockBack_distance;
    [Header("피격 후 무적시간")][SerializeField] private int InvincibleTime;


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
        // 방향키 입력 //
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // 이동속도 제한 //
        if (rigid.velocity.x > Speed)
        {
            rigid.velocity = new Vector2(Speed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < Speed * (-1))
        {
            rigid.velocity = new Vector2(Speed * (-1), rigid.velocity.y);
        }

        // 미끄럼 방지 //
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.01f, rigid.velocity.y);
        }

        // 좌우 반전 //
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // 대쉬 //
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(StaminaPoint > 50)
            {
                Debug.Log("대쉬");
                StaminaPoint -= 50;
            }
            else
            {
                Debug.Log("기력이 부족합니다.");
            }

        }
    }

    void Jump()
    {
        // 점프 //
        if (!Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            if(!isJump)
            {
                rigid.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            }
        }

        // 착지 시 jumpNum = 0 초기화 //


        // 하향 점프
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("하향 점프");
            // 하향 점프 속도는 점프력(JumpForce)값에 비례하지 않고 중력(Gravity)값에 비례한다.
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

                Debug.Log("공격");
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
            Debug.Log("더블 점프 가능해짐");
            canDoubleJump = true;
            Destroy(collision.gameObject);
        }
    }
}
