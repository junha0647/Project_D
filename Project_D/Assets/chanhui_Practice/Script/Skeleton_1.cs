using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_1 : Monster
{
    public enum State
    {
        Idle,
        Run,
        Attack,
        Follow,
        Ready,
    };
    public State currentState = State.Idle;

    public Transform[] wallCheck;

    WaitForSeconds Delay1500 = new WaitForSeconds(1.5f);
    WaitForSeconds Delay3000 = new WaitForSeconds(3.0f);

    private new void Awake()
    {
        base.Awake();
        moveSpeed = 2f;
        currentHp = 10;
        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;
        followRange = 10f;
        AtkRange = 1f;


        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    IEnumerator Idle()
    {
        yield return null;
        
        //Debug.Log("Idle");
        yield return Delay1500;

        if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < followRange && !IsPlayerDir())
        {
            MyAnimSetBool("IsIdle", false);
            MyAnimSetBool("IsRun", true);
            currentState = State.Follow;
        }
        else
        {
            MyAnimSetBool("IsIdle", false);
            MyAnimSetBool("IsWalk", true);
            currentState = State.Run;
        }
    }

    IEnumerator Run()
    {
        yield return null;
        float runTime = Random.Range(2f, 3f); // 랜덤 걸어다니는 시간
        //Debug.Log("Run");
        
        while (runTime >= 0f)
        {
            runTime -= Time.deltaTime;
            
            if (!isHit)
            {
                
                rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

                if (canAtk && IsPlayerDir() && isGround)
                {
                    // 플레이어가 사거리 안에 있는지
                    if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < followRange)
                    {
                        MyAnimSetBool("IsWalk", false);
                        Debug.Log("제발");
                        currentState = State.Follow;
                        break;
                    }
                }

                if (Physics2D.OverlapCircle(wallCheck[0].position, 0.01f, layerMask[0]))
                {
                    MonsterFlip();
                }
  
            }
            yield return null;
        }
        
        if (currentState != State.Attack)
        {
            MyAnimSetBool("IsWalk", false);
            MyAnimSetBool("IsIdle", true);
            //Debug.Log("멈춤");
            currentState = State.Idle;
        }
        
    }

    IEnumerator Follow()
    {
        yield return null;
        Debug.Log("Follow");
        if (!isHit)
        {
            
            if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) <= AtkRange)
            {
                
                currentState = State.Attack; // 공격
                //Debug.Log("공격함");

            }
            else if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) <= followRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, PlayerData.Instance.Player.transform.position, Time.deltaTime * moveSpeed);
                //Debug.Log("쫓아가");
                if(!IsPlayerDir())
                {
                    MonsterFlip();
                }
            }
            else if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) >= followRange)
            {
                MyAnimSetBool("IsRun", false);
                MyAnimSetBool("IsWalk", true);
                currentState = State.Run;
            }
  
        }
        
    }

    IEnumerator Attack()
    {
        yield return null;
        //Debug.Log("Attack");
        if (!isHit && isGround && (atkCoolTimeCalc == 3f))
        {
            MyAnimSetTrigger("IsAtk");
            canAtk = false;
            //Debug.Log("공격함");
            MyAnimSetBool("IsIdle", true);
            currentState = State.Ready;
        }
    }

    IEnumerator Ready()
    {
        yield return null;
        //Debug.Log("Ready");
        yield return Delay1500;
        if (Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < followRange)
        {
            MyAnimSetBool("IsIdle", false);
            MyAnimSetBool("IsRun", true);
            currentState = State.Follow;
            //Debug.Log("사거리 들어옴");
        }
        else
        {
            currentState = State.Idle;
        }
    }


    private void Update()
    {
        GroundCheck();

    }

    /*
    protected new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.transform.CompareTag("PlayerHitBox"))
        {
            MonsterFlip();
        }
    }*/
}
