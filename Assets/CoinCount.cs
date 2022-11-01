using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    int currentCoinCount;
    public TextMeshProUGUI coinText;

    private void Update()
    {
        if(currentCoinCount != BlackBoard.collectedCollectibles)
        {
            currentCoinCount = BlackBoard.collectedCollectibles;

            if (currentCoinCount < 10)
            {
                coinText.text = "0" + currentCoinCount.ToString();
            }
            else
            {
                coinText.text = currentCoinCount.ToString();
            }
        }
    }
}
