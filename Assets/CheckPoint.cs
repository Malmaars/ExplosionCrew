using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CheckPoint : MonoBehaviour
{
    public Transform spawnPoint;
    public ParticleSystem newCheckPoint;
    public EventReference checkpointSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && BlackBoard.CurrentCheckPoint != spawnPoint)
        {
            BlackBoard.CurrentCheckPoint = spawnPoint;
            newCheckPoint.Play();
            RuntimeManager.PlayOneShot(checkpointSound);
        }
    }
}
