using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Tooltip("몬스터 피")]
    public int currentHp = 1;
    [Tooltip("몬스터 이동 속도")]
    public float moveSpeed = 5f;

    public float jumpPower = 10;
    [Tooltip("공격 텀")]
    public float atkCoolTime = 3f;
    public float atkCoolTimeCalc = 3f;

    [Tooltip("플레이어게 닿았는지")]
    public bool isHit = false;
    [Tooltip("땅이 있는지")]
    public bool isGround = true;
    [Tooltip("공격을 했는지")]
    public bool canAtk = true;
    public bool MonsterDirRight = true;
    [Tooltip("추적 범위")]
    public float followRange;
    [Tooltip("공격 범위")]
    public float AtkRange;

    protected Rigidbody2D rb;
    protected CapsuleCollider2D CsCollider;
    public GameObject hitBoxCollider;
    public Animator Anim;

    [Header("플렛폼을 감지")]
    public LayerMask[] layerMask;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CsCollider = GetComponent<CapsuleCollider2D>();
        Anim = GetComponent<Animator>();


        StartCoroutine(CalcCoolTime());
        StartCoroutine(ResetCollider());
    }

    // 콜라이더를 리셋시키는 부분
    IEnumerator ResetCollider()
    {
        while (true)
        {
            yield return null;
            if (!hitBoxCollider.activeInHierarchy) // 공격을 하고 나서, HitBox를 다시 켜준다. ( 정한 시간이 지난 후)
            {
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }

    // 공격 쿨타임
    IEnumerator CalcCoolTime()
    {
        while (true)
        {
            yield return null;
            if (!canAtk)
            {
                atkCoolTimeCalc -= Time.deltaTime;
                if (atkCoolTimeCalc <= 0)
                {
                    atkCoolTimeCalc = atkCoolTime;
                    canAtk = true;
                }
            }
        }
    }

    public bool IsPlayingAnim(string AnimName)
    {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName))
        {
            return true;
        }
        return false;
    }

    // 플레이어를 바라보는 방향으로 Flip으로 바꾸어준다.
    protected void MonsterFlip()
    {
        MonsterDirRight = !MonsterDirRight;

        Vector3 thisScale = transform.localScale;
        if (MonsterDirRight)
        {
            thisScale.x = Mathf.Abs(thisScale.x);
        }
        else
        {
            thisScale.x = -Mathf.Abs(thisScale.x);
        }
        transform.localScale = thisScale;
        rb.velocity = Vector2.zero;
    }

    protected bool IsPlayerDir()
    {
        if (transform.position.x < PlayerData.Instance.Player.transform.position.x ? MonsterDirRight : !MonsterDirRight)
        {
            return true;
        }
        return false;
    }
    public void MyAnimSetBool(string AnimName, bool Is)
    {
        if (!IsPlayingAnim(AnimName))
        {
            Anim.SetBool(AnimName, Is);
        }
    }

    public void MyAnimSetTrigger(string AnimName)
    {
        if (!IsPlayingAnim(AnimName))
        {
            Anim.SetTrigger(AnimName);
        }
    }

    protected void GroundCheck()
    {

        if (Physics2D.BoxCast(CsCollider.bounds.center, CsCollider.size, 0, Vector2.down, 0.05f, layerMask[0]))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public void TakeDamage(int dam)
    {
        currentHp -= dam;
        isHit = true;
        hitBoxCollider.SetActive(false);

    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if(collision.transform.CompareTag())
        {
            TakeDamage(0);
        }*/
    }

}
