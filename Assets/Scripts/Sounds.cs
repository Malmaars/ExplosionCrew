using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Sounds : MonoBehaviour
{
    public EventReference footstepSound;
    public void PlayFootStep()
    {
        RuntimeManager.PlayOneShot(footstepSound);
    }
}
