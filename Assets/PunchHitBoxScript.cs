using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchHitBoxScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyV2>().Die();
        }
    }
}
