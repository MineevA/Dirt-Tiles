using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Spray : MonoBehaviour
{
    public GameObject jet;
    public GameObject stream;
    public GameObject splash;

    public Texture2D sprayWashPattern;
    public Texture2D streamWashPattern;

    private void Start()
    {

    }

    public void SetEffectActive(bool enabled)
    {
        jet.SetActive(enabled);
        stream.SetActive(enabled);
        splash.SetActive(enabled);
    }
    
    public void CleanDirt(Vector3 target, GunMode mode)
    {
        var ray = new Ray(target, Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<Dirt>(out var dirt))
            {
                dirt.DrawPixels(hit.textureCoord);
            }
        }
    }
}
