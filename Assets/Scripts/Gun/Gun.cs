using UnityEngine;

[RequireComponent(typeof(Spray))]
public class Gun : MonoBehaviour
{
    public bool isActivated;
    public Sprite streamSprite;
    public Sprite spraySprite;

    private Spray sprayEffect;

    private StreamGunMode streamGunMode;
    private SprayGunMode sprayGunMode;
    private bool isSprayMode;

    private void Start()
    {
        sprayEffect = GetComponent<Spray>();

        streamGunMode = new StreamGunMode();
        streamGunMode.SetSprite(streamSprite);

        sprayGunMode = new SprayGunMode();
        sprayGunMode.SetSprite(spraySprite);

        isSprayMode = true;

        sprayEffect.SetMode(sprayGunMode);
    }

    public void GunMoved()
    {
        sprayEffect.CleanNext();
    }

    public void SetActive(bool active)
    {
        isActivated = active;
        sprayEffect.SetEffectActive(isActivated);
    }

    public void ChangeMode()
    {
        isSprayMode = !isSprayMode;

        if (isSprayMode)
            sprayEffect.SetMode(sprayGunMode);
        else
            sprayEffect.SetMode(streamGunMode);
    }
}