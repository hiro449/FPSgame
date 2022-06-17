using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiCon : MonoBehaviour
{

    Animator animator;

    NavMeshAgent agent;

    public float walkingSpeed;

    enum STATE { IDLE, WANTED, ATTACK, CHASE, DEAD };
    STATE state = STATE.IDLE;

    GameObject target;
    public float runSpeed;

    public int attackDamage;




    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void TurnOffTrigger()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Death", false);
        animator.SetBool("Attack", false);
    }

    float DistancePlayer()
    {

        if (GameState.GameOver)
        {
            return Mathf.Infinity;
        }
        return Vector3.Distance(target.transform.position, transform.position);
    }

    bool CanSeePlayer()
    {
        if (DistancePlayer() < 15)
        {
            return true;
        }
        return false;
    }

    bool ForgetPlayer()
    {
        if (DistancePlayer() > 20)
        {
            return true;
        }
        return false;
    }

    public void DamagePlayer()
    {
        if (target != null)
        {
            target.GetComponent<FPSControler>().TakeHP(attackDamage);
        }
    }

    public void ZonbiDeath()
    {
        TurnOffTrigger();
        animator.SetBool("Death", true);
        state = STATE.DEAD;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                TurnOffTrigger();

                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                else if (Random.Range(0, 5000) < 5)
                {
                    state = STATE.WANTED;
                }
                break;
            case STATE.WANTED:
                if (!agent.hasPath)
                {
                    float newX = transform.position.x + Random.Range(-5, 5);
                    float newZ = transform.position.z + Random.Range(-5, 5);

                    Vector3 NextPos = new Vector3(newX, transform.position.y, newZ);
                    agent.SetDestination(NextPos);
                    agent.stoppingDistance = 0;

                    TurnOffTrigger();

                    agent.speed = walkingSpeed;
                    animator.SetBool("Walk", true);
                }

                if (Random.Range(0, 5000) < 5)
                {
                    state = STATE.IDLE;
                    agent.ResetPath();
                    state = STATE.WANTED;

                    return;
                }

                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                break;
            case STATE.ATTACK:

                if (GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();
                    state = STATE.WANTED;

                }
                TurnOffTrigger();
                animator.SetBool("Attack", true);

                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

                if (DistancePlayer() > agent.stoppingDistance + 2)
                {
                    state = STATE.CHASE;
                }

                break;
            case STATE.CHASE:

                if (GameState.GameOver)
                {
                    TurnOffTrigger();
                    agent.ResetPath();

                }

                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3;
                TurnOffTrigger();

                agent.speed = runSpeed;
                animator.SetBool("Run", true);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    state = STATE.ATTACK;
                }

                if (ForgetPlayer())
                {
                    agent.ResetPath();
                    state = STATE.WANTED;
                }
                break;
            case STATE.DEAD:

                Destroy(agent);
                break;
            default:
                break;
        }
    }
}
