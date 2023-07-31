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

}
