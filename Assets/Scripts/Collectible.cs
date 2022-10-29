using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //collect
            BlackBoard.collectedCollectibles++;
            Destroy(this.gameObject);
            Debug.Log(BlackBoard.collectedCollectibles);
        }
    }
}
