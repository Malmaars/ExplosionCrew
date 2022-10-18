using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class ImpactAnimation
{
    public static void PlayImpact(CinemachineVirtualCamera virtualCamera)
    {
        virtualCamera.Priority = 11;
        Time.timeScale = 0;   
    }
}
