using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyPattern : EnemyV2
{
    Transform player;

    public float moveSpeed;
    public float attackSpeed;
    public float distanceUntilAttack;
    public LayerMask ignoreThese;

    MeshRenderer meshRender;
    public Material MatAttack, MatIdle;
    NavMeshAgent agent;

    public Transform[] Wander;
    int currentWander;
    bool wandering;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRender = GetComponent<MeshRenderer>();
        player = FindObjectOfType<PlayerV2>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese));

        if(Vector3.Distance(transform.position, player.position) < distanceUntilAttack && !Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese))
        {
            if(agent.speed != attackSpeed)
            {
                agent.speed = attackSpeed;
            }
            //move to player
            meshRender.material = MatAttack;
            agent.SetDestination(player.position);
            wandering = false;
        }

        else
        {
            if (agent.speed != moveSpeed)
                agent.speed = moveSpeed;

            meshRender.material = MatIdle;

            if (!wandering)
            {
                agent.SetDestination(Wander[currentWander].position);
                wandering = true;
            }

            else
            {
                if(Vector3.Distance(transform.position, Wander[currentWander].position) < 1)
                {
                    currentWander++;

                    if (currentWander >= Wander.Length)
                        currentWander = 0;

                    agent.SetDestination(Wander[currentWander].position);
                }
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == " Player")
    //    {
    //        collision.transform.GetComponent<PlayerV2>().TakeDamage(1);
    //    }
    //}
}
