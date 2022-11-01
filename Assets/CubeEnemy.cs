using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEnemy : EnemyV2
{
    Transform player;
    Rigidbody rb;
    public LayerMask ignoreThese;
    Transform[] sides;

    List<Transform> sidesThatTouchGround;

    bool rolling;
    public float distanceUntilAttack;
    public Transform cubeParent;

    public float attackSpeed;
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerV2>().transform;
        sidesThatTouchGround = new List<Transform>();
        sides = new Transform[transform.childCount];
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < transform.childCount; i++)
        {
            sides[i] = transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sidesThatTouchGround.Clear();
        for (int i = 0; i < sides.Length; i++)
        {
            if (Physics.CheckSphere(sides[i].position, 0.1f, ~ignoreThese))
            {
                sidesThatTouchGround.Add(sides[i]);
            }
        }

        if (Vector3.Distance(transform.position, player.position) < distanceUntilAttack && !Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese))
        {
            RollOver();
        }

        //Debug.Log(sidesThatTouchGround.Count);
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < sides.Length; i++)
    //    {

    //        Gizmos.DrawSphere(sides[i].position, 0.1f);
    //    }
    //}

    void RollOver()
    {
        //Transform closestToPlayer = sidesThatTouchGround[0];
        //for(int i = 1; i < sidesThatTouchGround.Count; i++)
        //{
        //    if(Vector3.Distance(sidesThatTouchGround[i].position, player.position) < Vector3.Distance(closestToPlayer.position, player.position))
        //    {
        //        closestToPlayer = sidesThatTouchGround[i];
        //    }
        //}

        if (sidesThatTouchGround.Count > 0)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);
            rb.AddForce(direction * attackSpeed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }
}
