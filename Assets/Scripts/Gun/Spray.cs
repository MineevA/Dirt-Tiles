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

    private Vector2 lastHitCoord = Vector2.left;

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
        if (Raycast(target.transform.position,out var dirt, out var hit))
        {
            dirt.DrawPixels(hit.textureCoord, erasePatternSizeUV, solidDirtModifier);
            lastHitCoord = hit.textureCoord;
        }
    }

    public void CleanNext()
    {
        if (Raycast(target.transform.position, out var dirt, out var hit))
        {
            if (lastHitCoord == Vector2.left)
                dirt.DrawPixels(hit.textureCoord, erasePatternSizeUV, solidDirtModifier);
            else
                dirt.DrawLine(lastHitCoord, hit.textureCoord, erasePatternSizeUV, solidDirtModifier);

            lastHitCoord = hit.textureCoord;
        }
        else
            lastHitCoord = Vector2.left;
    }
    
    public void SetMode(IGunMode gunMode)
    {
        gunMode.OnEnable(this);
    }

    private bool Raycast(Vector3 target, out Dirt dirt, out RaycastHit hit)
    {
        var ray = new Ray(target, Vector3.forward);

        if (Physics.Raycast(ray, out hit))
            return hit.collider.TryGetComponent<Dirt>(out dirt);

        dirt = null;
        hit = new RaycastHit();
        return false;
    }

}
