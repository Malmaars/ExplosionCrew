using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyPattern : MonoBehaviour
{
    Transform player;

    public float movespeed;
    public float distanceUntilAttack;
    public LayerMask ignoreThese;

    MeshRenderer meshRender;
    public Material MatAttack, MatIdle;
    // Start is called before the first frame update
    void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        player = FindObjectOfType<PlayerV2>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese));

        if(Vector3.Distance(transform.position, player.position) < distanceUntilAttack && !Physics.Raycast(transform.position, (player.position - transform.position).normalized, Vector3.Distance(transform.position, player.position), ~ignoreThese))
        {
            //move to player
            meshRender.material = MatAttack;
        }

        else
        {
            meshRender.material = MatIdle;
        }
    }
}
