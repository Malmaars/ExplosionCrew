using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class FinalCollectible : MonoBehaviour
{
    PlayerV2 player;
    public CinemachineDollyCart cart;
    public CinemachineVirtualCamera endCamera;
    public float speed;
    int healthAtPickup;

    public Canvas endCanvas;
    public RawImage black;

    private void Start()
    {
        player = FindObjectOfType<PlayerV2>();

    }

    private void Update()
    {
        if (cart.enabled == true)
        {
            //cart.m_Position += speed;
            player.playerHealth = healthAtPickup;
            float alpha = black.color.a + Time.deltaTime * speed;
            Color currColor = black.color;
            currColor.a = alpha;
            black.color = currColor;

            if(black.color.a >= 1)
            {
                Application.Quit();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //player.Die();
            healthAtPickup = player.playerHealth;
            cart.enabled = true;
            endCamera.Priority = 20;
            endCanvas.gameObject.SetActive(true);
            //initiate final scene
        }
    }
}
