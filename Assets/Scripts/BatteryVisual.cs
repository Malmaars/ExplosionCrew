using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryVisual : MonoBehaviour
{
    PlayerV2 player;
    int currentCharge;
    RawImage[] Charges;
    

    private void Awake()
    {
        player = FindObjectOfType<PlayerV2>();
        Charges = new RawImage[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Charges[i] = transform.GetChild(i).GetComponent<RawImage>();
        }
    }

    private void Update()
    {
        if(currentCharge != player.airBoostCount)
        {
            currentCharge = player.airBoostCount;

            for(int i = 0; i < Charges.Length; i++)
            {
                if(i + 1 > currentCharge)
                {
                    Charges[i].color = Color.red;
                }

                else
                {
                    Charges[i].color = Color.green;
                }
            }
        }
    }
}
