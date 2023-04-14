using UnityEngine;

public class Spray : MonoBehaviour
{
    public GameObject jet;
    public GameObject stream;
    public GameObject splash;
    public GameObject target;
    public GameObject particles;

    public Vector2 erasePatternSizeUV;
    public float solidDirtModifier;

    private IGunMode gunMode;

    public void SetEffectActive(bool enabled)
    {
        jet.SetActive(enabled);
        stream.SetActive(enabled);
        splash.SetActive(enabled);

        if (enabled)
            CleanDirt();
    }
    
    public void CleanDirt()
    {
        gunMode.OnCleanDirt(this);
    }

    public void CleanNext()
    {
        gunMode.OnCleanNext(this);
    }

    public void SetMode(IGunMode gunMode)
    {
        this.gunMode = gunMode;
        gunMode.OnEnable(this);
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

}
