using UnityEngine;

[RequireComponent(typeof(Spray))]
public class Gun : MonoBehaviour
{
    public Transform target;
    
    private GunMode gunMode;
    public GunMode GunMode
    {
        get => gunMode;
        set => gunMode = value;
    }

    private bool isActivated = false;
    public bool IsActivated 
    {
        get => isActivated;
        set
        {
            isActivated = value; 
            sprayEffect.SetEffectActive(isActivated);
            if (isActivated)
                sprayEffect.CleanDirt(target.position);
        }
    }

    private Spray sprayEffect;

    private void Start()
    {
        sprayEffect = GetComponent<Spray>();
        gunMode = GunMode.Spray;
    }

    public void GunMoved()
    {
        sprayEffect.CleanNext(target.position);
    }
}

public enum GunMode
{
    Spray, 
    Stream
}
