using UnityEngine;

public class StreamGunMode : IGunMode
{
    private readonly Vector2 targetPosition = new(-1.15f, 2.13f);
    private readonly Vector2 splashPosition = new(-1.04f, 4.14f);
    private readonly Vector2 splashScale = new(0.18f, 1f);
    private readonly Vector2 streamPosition = new(-1.04f, 6.23f);
    private readonly Vector2 streamScale = new(0.12f, 1f);
    private readonly Vector2 patternRelativeSize = new(0.02f, 0.02f);
    private Sprite jetSprite;
    
    public void OnEnable(Spray spray)
    {
        SetTransform(spray.target.transform, targetPosition, Vector2.one);
        SetTransform(spray.splash.transform, splashPosition, splashScale);
        SetTransform(spray.stream.transform, streamPosition, streamScale);

        spray.jet.GetComponent<SpriteRenderer>().sprite = jetSprite;
        
        spray.erasePatternSizeUV = patternRelativeSize;
        
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
