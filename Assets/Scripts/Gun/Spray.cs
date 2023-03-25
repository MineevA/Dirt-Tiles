using UnityEngine;

public class Spray : MonoBehaviour
{
    public GameObject jet;
    public GameObject stream;
    public GameObject splash;

    private Vector2 lastHitCoord = Vector2.left;

    public void SetEffectActive(bool enabled)
    {
        jet.SetActive(enabled);
        stream.SetActive(enabled);
        splash.SetActive(enabled);
    }
    
    public void CleanDirt(Vector3 target)
    {
        if (Raycast(target,out var dirt, out var hit))
        {
            dirt.DrawPixels(hit.textureCoord);
            lastHitCoord = hit.textureCoord;
        }
    }

    public void CleanNext(Vector3 target)
    {
        if (Raycast(target, out var dirt, out var hit))
        {
            if (lastHitCoord == Vector2.left)
                dirt.DrawPixels(hit.textureCoord);
            else
                dirt.DrawLine(lastHitCoord, hit.textureCoord);

            lastHitCoord = hit.textureCoord;
        }
        else
            lastHitCoord = Vector2.left;
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
