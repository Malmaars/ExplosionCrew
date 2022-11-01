using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class introtrack : MonoBehaviour
{
    public CinemachineDollyCart cart;
    PlayerV2 player;

    public CinemachineVirtualCamera thisCamera;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerV2>();
        player.intro = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(cart.m_Position >= 1)
        {
            thisCamera.Priority = 0;
            player.intro = false;
        }
    }
}
