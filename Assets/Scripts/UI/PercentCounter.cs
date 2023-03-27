using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PercentCounter : MonoBehaviour
{
    private Dictionary<DirtCounter, float> countersMap;
    private TMP_Text text;
    private float counter;

    public event Action<float> OnMainCounterUpdate;
    
    void Start()
    {
        text = GetComponent<TMP_Text>();
        
        var dirtGameObjects = GameObject.FindGameObjectsWithTag("Dirt");
        countersMap = new Dictionary<DirtCounter, float>();
        
        foreach(var dirt in dirtGameObjects)
        {
            var dirtCounter = dirt.GetComponent<DirtCounter>();
            dirtCounter.OnCounterUpdate += OnDirtCounterUpdate;
            countersMap.Add(dirtCounter, 0f);
        }
    }
    
    private void OnDirtCounterUpdate(int pixelsLeft, int pixelsTotal, DirtCounter dirtCounter)
    {
        countersMap[dirtCounter] = 1.0f - (float)pixelsLeft / pixelsTotal;

        ReCalculateCounters();
        DrawCounter();
    }

    private void ReCalculateCounters()
    {
        counter = 0f;
        
        foreach(var keyValue in countersMap)
            counter += keyValue.Value * 100;

        OnMainCounterUpdate?.Invoke(counter);
    }

    private void DrawCounter()
    {
        text.text = counter.ToString("F");
    }
}
