using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EnemyV2 : MonoBehaviour
{
    public EventReference deathSound;
    GameObject deathParticles;
    public int damageAmount;
    public void Die()
    {
        RuntimeManager.PlayOneShot(deathSound);
        deathParticles = Resources.Load("DeathParticles") as GameObject;

        Instantiate(deathParticles, transform.position, new Quaternion(0, 0, 0, 0));
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerV2 player = collision.transform.GetComponent<PlayerV2>();
            player.TakeDamage(damageAmount, collision.contacts[0].point);
        }
    }
}
