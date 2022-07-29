using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Tooltip("���� ��")]
    public int currentHp = 1;
    [Tooltip("���� �̵� �ӵ�")]
    public float moveSpeed = 5f;

    public float jumpPower = 10;
    [Tooltip("���� ��")]
    public float atkCoolTime = 3f;
    public float atkCoolTimeCalc = 3f;

    [Tooltip("�÷��̾�� ��Ҵ���")]
    public bool isHit = false;
    [Tooltip("���� �ִ���")]
    public bool isGround = true;
    [Tooltip("������ �ߴ���")]
    public bool canAtk = true;
    public bool MonsterDirRight = true;
    [Tooltip("���� ����")]
    public float followRange;
    [Tooltip("���� ����")]
    public float AtkRange;

    protected Rigidbody2D rb;
    protected CapsuleCollider2D CsCollider;
    public GameObject hitBoxCollider;
    public Animator Anim;

    [Header("�÷����� ����")]
    public LayerMask[] layerMask;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CsCollider = GetComponent<CapsuleCollider2D>();
        Anim = GetComponent<Animator>();


        StartCoroutine(CalcCoolTime());
        StartCoroutine(ResetCollider());
    }

    // �ݶ��̴��� ���½�Ű�� �κ�
    IEnumerator ResetCollider()
    {
        while (true)
        {
            yield return null;
            if (!hitBoxCollider.activeInHierarchy) // ������ �ϰ� ����, HitBox�� �ٽ� ���ش�. ( ���� �ð��� ���� ��)
            {
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }

    // ���� ��Ÿ��
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

    // �÷��̾ �ٶ󺸴� �������� Flip���� �ٲپ��ش�.
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
