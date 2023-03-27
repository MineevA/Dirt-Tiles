using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtParticles : MonoBehaviour
{
    public int particlesPerPercent = 10;
    
    private PercentCounter mainCounter;
    private ParticleSystem particles;
    private float lastCounter = 0;

    private void Start()
    {
        mainCounter = GameObject.FindGameObjectWithTag("Main counter").GetComponent<PercentCounter>();
        mainCounter.OnMainCounterUpdate += OnMainCounterUpdate;

        particles = GetComponent<ParticleSystem>();
    }

    private void OnMainCounterUpdate(float counter) 
    { 
        var emitCount = (int)Mathf.Max(1f,(counter - lastCounter) * particlesPerPercent);
        particles.Emit(emitCount);
        lastCounter = counter;
    }
}
