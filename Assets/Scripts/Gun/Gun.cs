using UnityEngine;

[RequireComponent(typeof(Spray))]
public class Gun : MonoBehaviour
{
    public bool inControl;
    public Sprite streamSprite;
    public Sprite spraySprite;
    public GameObject target;
    public GameObject particles;

    private Spray sprayEffect;
    private StreamGunMode streamGunMode;
    private SprayGunMode sprayGunMode;

    public Vector2 erasePatternSizeUV;
    public float solidDirtModifier;

    private IGunMode currentMode;

    private void Start()
    {
        sprayEffect = GetComponent<Spray>();

        streamGunMode = new StreamGunMode();
        streamGunMode.SetSprite(streamSprite);

        sprayGunMode = new SprayGunMode();
        sprayGunMode.SetSprite(spraySprite);

        currentMode = sprayGunMode;
        inControl = false;

        currentMode.OnEnable(this, sprayEffect);
    }

    public void GunMoved(Vector3 position)
    {
        currentMode.OnMove(position, this);
    }

    public void SetActive(bool active, Vector3 position)
    {
        inControl = active;
        currentMode.OnSetActive(active, this, position);
        sprayEffect.SetEffectActive(active);
    }

    public void ChangeMode()
    {
        currentMode.OnDisable();
        
        if (currentMode == sprayGunMode)
            currentMode = streamGunMode;
        else
            currentMode = sprayGunMode;

        currentMode.OnEnable(this, sprayEffect);
    }

    public Vector2 TryDrawPixels()
    {
        return TryDrawPixels(Vector2.zero);
    }

    public Vector2 TryDrawPixels(Vector2 modificator)
    {
        if (Raycast(target.transform.position, out var dirt, out var hit))
        {
            dirt.DrawPixels(hit.textureCoord + modificator,
                            erasePatternSizeUV,
                            solidDirtModifier);

            return hit.textureCoord;
        }

        return Vector2.left;
    }

    public Vector2 TryDrawLine(Vector2 startPoint)
    {
        if (Raycast(target.transform.position, out var dirt, out var hit))
        {
            dirt.DrawLine(startPoint,
                          hit.textureCoord,
                          erasePatternSizeUV,
                          solidDirtModifier);

            return hit.textureCoord;
        }
        
        return Vector2.left;
    }

    public bool Raycast(Vector3 target, out Dirt dirt, out RaycastHit hit)
    {
        var ray = new Ray(target, Vector3.forward);

        if (Physics.Raycast(ray, out hit))
            return hit.collider.TryGetComponent<Dirt>(out dirt);

        dirt = null;
        hit = new RaycastHit();
        return false;
    }

    private void Update()
    {
        currentMode.OnFrameUpdate();
    }
}