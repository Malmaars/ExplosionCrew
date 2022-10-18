using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public TextMeshProUGUI UINumber;
    public void UpdateText(string text)
    {
        UINumber.text = text;
    }
}
