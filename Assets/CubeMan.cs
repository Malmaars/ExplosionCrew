using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMan : EnemyV2
{
    Transform player;
    Rigidbody rb;

    public float attackSpeed;
    public float maxSpeed;
    public float distanceUntilAttack;

    public LayerMask ignoreThese;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerV2>().transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < distanceUntilAttack && !Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese))
        {
            //roll towards player
            //transform.forward = Vector3.Lerp(transform.rotation.eulerAngles, (transform.position - player.position).normalized, 0.5f);

            Vector3 direction = (player.position - transform.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);
            rb.AddForce(direction * attackSpeed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }
}
