using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
}


public class NPC : MonoBehaviour, IDamagable
{
    [Header("Status")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    NavMeshAgent agent;
    public float detectDistance;
    AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    float lastAttackTime;
    public float attackDistance;

    float playerDistance;
    public float fieldOfView = 120f;

    Animator animator;
    SkinnedMeshRenderer[] meshRenderers;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, 
            CharacterManager.Instance.Player.transform.position);
        
        animator.SetBool("Moving", aiState != AIState.Idle);
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void SetState(AIState state)
    {
        aiState = state;
        
        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;

            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;

            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", 
                Random.Range(minWanderWaitTime, maxWanderWaitTime)); 
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }
    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }
    //Vector3 GetWanderLocation(int repeat)
    //{
    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(
    //        transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), 
    //        out hit, maxWanderDistance, NavMesh.AllAreas);

    //    if (Vector3.Distance(transform.position, hit.position) < detectDistance)
    //    {
    //        return GetWanderLocation(repeat - 1);
    //    }

    //    return hit.position;
    //}
    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }
    void AttackingUpdate()
    {
        if (playerDistance < attackDistance && IsPlayerInFOV())
        {
            agent.isStopped = true;
            if(Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if (playerDistance <detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if(agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFOV()
    {
        Vector3 directionToTplayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTplayer);
        return angle < fieldOfView * 0.5f;
    }

    public void TakePhysicalDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();

        //데미지 효과
        StartCoroutine(DamageFlash());
    }
    void Die()
    {
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white;
        }
    }
}
