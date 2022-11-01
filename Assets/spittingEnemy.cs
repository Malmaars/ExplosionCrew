using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spittingEnemy : EnemyV2
{
    public GameObject spitPrefab;

    Transform player;
    public Transform muzzle;
    public float distanceUntilAttack;
    public float shotPower;
    public float timeBetweenShots;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerV2>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(Vector3.Distance(player.position, transform.position) < distanceUntilAttack)
        {
            //spit
            Vector3 newDirection = player.position - transform.position;
            newDirection = new Vector3(newDirection.x, 0, newDirection.z);
            transform.forward = Vector3.Lerp(transform.forward, newDirection, 0.3f);

            if(timer >= timeBetweenShots)
            {
                //fire
                timer = 0;
                Spit();
            }
        }
    }

    void Spit()
    {
        GameObject temp = Instantiate(spitPrefab, muzzle.position, new Quaternion(0, 0, 0, 0));

        temp.GetComponent<Rigidbody>().AddForce((player.position - muzzle.position).normalized * shotPower, ForceMode.Impulse);
    }
}
