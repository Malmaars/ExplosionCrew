using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Enemy : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    PlayerMovement player;
    bool startAnimation;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (startAnimation)
        {
            spinCamera();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(collision.transform.GetComponent<PlayerMovement>().movementSpeed > collision.transform.GetComponent<PlayerMovement>().maxMovementSpeed + 1)
            {
                player.enabled = false;
                startAnimation = true;
                //StartCoroutine(rotateCamera(0.001f));
            }
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    IEnumerator rotateCamera(float waitTime)
    {
        Debug.Log("die and give a cool animation");
        virtualCamera.Priority = 11;
        Time.timeScale = 0.001f;

        yield return new WaitForSeconds(waitTime);
        
        virtualCamera.Priority = 1;
        Time.timeScale = 1;
    }

    void spinCamera()
    {
        virtualCamera.Priority = 11;
        Time.timeScale = 0.001f;

        virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_Heading.m_Bias += 10;

        if(virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_Heading.m_Bias > 180)
        {
            player.enabled = true;
            virtualCamera.Priority = 0;
            Time.timeScale = 1f;
            Die();
        }
    }
}
