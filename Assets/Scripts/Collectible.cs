using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Collectible : MonoBehaviour
{
    PlayerV2 player;
    public EventReference coin;
    public ParticleSystem coinParticles;

    private void Start()
    {
        player = FindObjectOfType<PlayerV2>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //collect
            CoinSound();
            BlackBoard.collectedCollectibles++;
            //Destroy(this.gameObject);
            GetComponentInChildren<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            if(player.playerHealth < 4)
            {
                player.playerHealth++;
            }
            //Debug.Log(BlackBoard.collectedCollectibles);
            coinParticles.Play();
        }
    }

    public void CoinSound()
    {
        RuntimeManager.PlayOneShot(coin);
    }
}
