using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("공격력")][SerializeField] private float OffensePower;
    [Header("방어력")][SerializeField] private float DefensePower;
    [Header("체력")][SerializeField] private float HealthPoint;
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
    Animator animator;

    // 찬희가 건들인 부분
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

    // === 기본 이동 로직 : 방향키 입력 === //
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // === 이동속도 제한 === //
        if (rigid.velocity.x > Speed)
        {
            rigid.velocity = new Vector2(Speed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < Speed * (-1))
        {
            rigid.velocity = new Vector2(Speed * (-1), rigid.velocity.y);
        }

        // === 이동 시 미끄럼 방지 === //
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.01f, rigid.velocity.y);
        }

        // === 좌우 반전 === //
        if (Input.GetAxis("Horizontal") < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        // === 걷기 애니메이션 === // 
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            animator.SetBool("isRun", false);
        }
        else
        {
            animator.SetBool("isRun", true);
        }

        // === 대쉬 === //
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (StaminaPoint > 50)
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

    // === 점프 === //
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

        // 착지 시 jumpNum = 0 초기화 //

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
            // 여기서 주의할 점은 rayHit의 2f와 distance의 범위(1.2f)를 잘 조절해야 함.
        }


        // === 하향 점프 === //
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("하향 점프");
            // 하향 점프 속도는 점프력(JumpForce)값에 비례하지 않고 중력(Gravity)값에 비례한다.
        }
    }

    float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;
    
    // 찬희가 건드림
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
                        // 근접 공격 데미지 공식 (몬스터 방어력 - (공격력(Offense Power) * 1))
                        // collider.GetComponentInChildren<Enemy1>().Damage(attackDetails);
                        // collider.GetComponent<EnemyController>().TakeDamage(OffensePower);
                        // 해당 오브젝트 스크립트에서 함수 인자에서 def 빼주는 걸로 해결.
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
    
    // === 플레이어 공격 범위 디버그 === //
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    // === 플레이어와 다른 오브젝트 상호작용 === //
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "doubleJumpItem")
        {
            Debug.Log("더블 점프 가능해짐");
            canDoubleJump = true;
            Destroy(collision.gameObject);
        }
    }

    private void Damage(AttackDetails details)
    {
        HealthPoint -= details.damageAmount;
    }
}