using System;
using UnityEngine;

public class DirtCounter : MonoBehaviour
{
    private Material dirtMap;
    private RenderTexture counterTex;
    private Texture2D cpuTexture;
    private Rect cpuTextureRect;

    private int pixelsTotal = 0;
    private int pixelsLeft = 0;

    private bool initialized = false;

    public int mapWidth, mapHeight;
    public byte alphaThreshold = 50;

    public event Action<int, int, DirtCounter> OnCounterUpdate;

    private void LazyInit()
    {
        dirtMap = GetComponent<MeshRenderer>().material;

        counterTex = new RenderTexture(mapWidth, mapHeight, 0);
        counterTex.Create();

        cpuTexture = new Texture2D(mapWidth, mapHeight);
        cpuTextureRect = new Rect(0f, 0f, mapWidth, mapHeight);

        initialized = true;
    }

    public void Recalculate()
    {
        if (!initialized)
            LazyInit();
        
        Graphics.Blit(dirtMap.mainTexture, counterTex, dirtMap);
        RenderTexture.active = counterTex;

        cpuTexture.ReadPixels(cpuTextureRect, 0, 0, false);
        var currentPixels = VisiblePixels(cpuTexture);

        RenderTexture.active = null;
        counterTex.Release();

        if (currentPixels != pixelsLeft)
        {
            if (currentPixels > pixelsTotal)
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
            if (pixel.a > alphaThreshold)
                visiblePixels++;

        return visiblePixels;
    }

    public void InitCounter()
    {
        pixelsTotal = 0;
        Recalculate();
    }
}
