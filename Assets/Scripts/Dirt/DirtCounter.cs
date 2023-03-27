using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DirtCounter : MonoBehaviour
{
    private Material dirtMap;
    private Texture sourceTexture;
    private RenderTexture counterTex;
    private Texture2D cpuTexture;
    private Rect cpuTextureRect;

    private int pixelsTotal = 0;
    private int pixelsLeft = 0;

    public int mapWidth, mapHeight;
    public byte alphaThreshold = 50;

    public event Action<int, int, DirtCounter> OnCounterUpdate;
    
    void Start()
    {
        dirtMap = GetComponent<MeshRenderer>().material; 
        sourceTexture = dirtMap.mainTexture;
        
        counterTex = new RenderTexture(mapWidth, mapHeight, 0);
        counterTex.Create();

        cpuTexture = new Texture2D(mapWidth, mapHeight);
        cpuTextureRect = new Rect(0f, 0f, mapWidth, mapHeight);
    }

    public void Recalculate()
    {
        Graphics.Blit(sourceTexture, counterTex, dirtMap);
        RenderTexture.active = counterTex;

        cpuTexture.ReadPixels(cpuTextureRect, 0, 0, false);
        var currentPixels = VisiblePixels(cpuTexture);

        RenderTexture.active = null;
        counterTex.Release();

        if (currentPixels != pixelsLeft)
        {
            if (pixelsTotal == 0)
                pixelsTotal = currentPixels;

            pixelsLeft = currentPixels;

            OnCounterUpdate?.Invoke(pixelsLeft, pixelsTotal, this);
        }
    }

    private int VisiblePixels(Texture2D texture)
    {
        int visiblePixels = 0;
        var pixels = texture.GetPixels32();
       
        foreach(var pixel in pixels)
            if (pixel.a >= alphaThreshold)
                visiblePixels++;

        return visiblePixels;
    }
}
