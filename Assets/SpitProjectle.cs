using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitProjectle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.transform.GetComponent<PlayerV2>().TakeDamage(1, transform.position);
        }
        Destroy(this.gameObject);
    }
}
