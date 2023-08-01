using System;
using UnityEngine;

public class Spray : MonoBehaviour
{
    public GameObject jet;
    public GameObject stream;
    public GameObject splash;

    public void SetEffectActive(bool enabled)
    {
        jet.SetActive(enabled);
        stream.SetActive(enabled);
        splash.SetActive(enabled);
    }

    public void SetEffectActive(EffectMask mask)
    {
        jet.SetActive(mask.HasFlag(EffectMask.Jet));
        stream.SetActive(mask.HasFlag(EffectMask.Stream));
        splash.SetActive(mask.HasFlag(EffectMask.Splash));
    }
}

[Flags]
public enum EffectMask
{
    None = 0,
    Jet = 1,
    Stream = 2,
    Splash = 4
}
