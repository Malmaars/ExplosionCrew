using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : EnemyV2
{
    Transform player;
    Rigidbody rb;

    float distToGround;

    public float moveSpeed;
    public float attackSpeed;
    public float maxSpeed;
    public float distanceUntilAttack;
    public LayerMask ignoreThese;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerV2>().transform;
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceUntilAttack && !Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese) && IsGrounded())
        {
            //roll towards player
            //transform.forward = Vector3.Lerp(transform.rotation.eulerAngles, (transform.position - player.position).normalized, 0.5f);

            Vector3 direction = (player.position - transform.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);
            rb.AddForce(direction * attackSpeed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
