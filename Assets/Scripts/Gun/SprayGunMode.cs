using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SprayGunMode : IGunMode
{
    private readonly Vector2 targetPosition = new(-1.55f, 2.1f);
    private readonly Vector2 splashPosition = new(-1.11f, 4.13f);
    private readonly Vector2 splashScale = new(1f, 1f);
    private readonly Vector2 streamPosition = new(-1.13f, 6.23f);
    private readonly Vector2 streamScale = new(1f, 2f);
    private readonly Vector2 patternRelativeSize = new(0.10f, 0.02f);
    private readonly float solidModifier = 0.1f;

    private Sprite jetSprite;
    private Vector2 lastHitCoord = Vector2.left;

    public void OnCleanDirt(Spray spray)
    {
        if (spray.Raycast(spray.target.transform, out var dirt, out var hit))
        {
            dirt.DrawPixels(hit.textureCoord,
                        spray.erasePatternSizeUV,
                        spray.solidDirtModifier);

            lastHitCoord = hit.textureCoord;
        }
    }

    public void OnCleanNext(Spray spray)
    {
        if (spray.Raycast(spray.target.transform, out var dirt, out var hit))
        {
            if (lastHitCoord == Vector2.left)
                dirt.DrawPixels(hit.textureCoord, 
                                spray.erasePatternSizeUV, 
                                spray.solidDirtModifier);
            else
                dirt.DrawLine(lastHitCoord, 
                              hit.textureCoord, 
                              spray.erasePatternSizeUV, 
                              spray.solidDirtModifier);

            lastHitCoord = hit.textureCoord;
        }
        else
            lastHitCoord = Vector2.left;
    }

    public void OnEnable(Spray spray)
    {
        SetTransform(spray.target.transform, targetPosition, Vector2.one);
        SetTransform(spray.splash.transform, splashPosition, splashScale);
        SetTransform(spray.stream.transform, streamPosition, streamScale);

        spray.jet.GetComponent<SpriteRenderer>().sprite = jetSprite;
        
        spray.erasePatternSizeUV = patternRelativeSize;
        spray.solidDirtModifier = solidModifier;

        lastHitCoord = Vector2.left;
    }

    public void SetSprite(Sprite gunModeSprite)
    {
        jetSprite = gunModeSprite;
    }

    private void SetTransform(Transform transform, Vector2 position, Vector2 scale)
    {
        transform.localPosition = new Vector3(position.x,
                                         position.y,
                                         transform.position.z);

        transform.localScale = new Vector3(scale.x,
                                           scale.y,
                                           transform.localScale.z);
    }
}
