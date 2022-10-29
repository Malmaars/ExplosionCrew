using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyV2 : MonoBehaviour
{
    public int damageAmount;
    public void Die()
    {
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
