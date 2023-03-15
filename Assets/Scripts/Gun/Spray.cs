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

    private Dictionary<GunMode, bool[,]> washPatternsAlphaFlagArrayMap;

    private void Start()
    {
        washPatternsAlphaFlagArrayMap = new Dictionary<GunMode, bool[,]>()
        {
            { GunMode.Spray, TextureAlphaFlagArray(sprayWashPattern) },
            { GunMode.Stream, TextureAlphaFlagArray(streamWashPattern) }
        };
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
                dirt.DrawPixels(hit.textureCoord, washPatternsAlphaFlagArrayMap[mode]);
            }
        }
    }

    private bool[,] TextureAlphaFlagArray(Texture2D texture)
    {
        var array = new bool[texture.width, texture.height];

        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                array[x, y] = texture.GetPixel(x, y).a > 0;

        return array;
    }
}
