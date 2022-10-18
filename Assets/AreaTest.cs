using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMODUnityResonance;

public class AreaTest : MonoBehaviour
{
    public StudioEventEmitter emitter;
    private void Update()
    {
        float whatIsTheArea;
        emitter.EventInstance.getParameterByName("Area", out whatIsTheArea);
        UnityEngine.Debug.Log(whatIsTheArea);
    }
}
