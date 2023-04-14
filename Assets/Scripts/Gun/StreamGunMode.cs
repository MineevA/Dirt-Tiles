using UnityEngine;

public class StreamGunMode : IGunMode
{
    private readonly Vector2 targetPosition = new(-1.15f, 2.13f);
    private readonly Vector2 splashPosition = new(-1.04f, 4.14f);
    private readonly Vector2 splashScale = new(0.18f, 1f);
    private readonly Vector2 streamPosition = new(-1.04f, 6.23f);
    private readonly Vector2 streamScale = new(0.12f, 1f);
    private readonly Vector2 patternRelativeSize = new(0.02f, 0.02f);
    private readonly float solidModifier = 0.6f;

    private Sprite jetSprite;
    private Vector2 lastHitCoord = Vector2.left;

    public void OnCleanDirt(Spray spray)
    {
        var target = spray.target.transform.position;

        if (spray.Raycast(target, out var dirt, out var hit))
            target = CleanTargetPoint(dirt, spray);

        if (spray.Raycast(target, out dirt, out hit))
        { 
            dirt.DrawPixels(hit.textureCoord,
                            spray.erasePatternSizeUV,
                            spray.solidDirtModifier);

            lastHitCoord = hit.textureCoord;
        }
    }

    public void OnCleanNext(Spray spray)
    {
        var target = spray.target.transform.position;

        if (spray.Raycast(target, out var dirt, out var hit))
            target = CleanTargetPoint(dirt, spray);

        if (spray.Raycast(target, out dirt, out hit))
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

    private Vector3 CleanTargetPoint(Dirt dirt, Spray spray)
    {
        var currentTargetPosition = spray.target.transform.position;
        var patternHalfSize = new Vector3(patternRelativeSize.x * dirt.transform.localScale.x / 2,
                                          patternRelativeSize.y * dirt.transform.localScale.y / 2);

        return dirt.navigationMap.ClosestPointToWorldPosition(currentTargetPosition) - patternHalfSize;
    }
}
