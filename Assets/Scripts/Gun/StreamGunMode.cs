using UnityEngine;
using UnityEngine.UIElements;

public class StreamGunMode : IGunMode
{
    private readonly Vector2 targetPosition = new(-1.15f, 2.13f);
    private readonly Vector2 splashPosition = new(-1.04f, 4.14f);
    private readonly Vector2 splashScale = new(0.18f, 1f);
    private readonly Vector2 streamPosition = new(-1.04f, 6.23f);
    private readonly Vector2 streamScale = new(0.12f, 1f);
    private readonly Vector2 patternRelativeSize = new(0.02f, 0.02f);
    private readonly float solidModifier = 1f;
    private readonly float moveStep = 0.1f;

    private Sprite jetSprite;
    private Gun gun;
    private NavigationMap navigationMap;

    private bool isMoving = false;
    private Vector3 moveDestination;
    private Vector3 catchModificator;
    private Vector2 patternDrawModificator;

    public void OnEnable(Gun gun, Spray spray)
    {
        this.gun = gun;
        gun.erasePatternSizeUV = patternRelativeSize;
        gun.solidDirtModifier = solidModifier;

        LazyNavigationMapInit();
        UpdateTransform(gun.target.transform, targetPosition, Vector2.one);
        UpdateTransform(spray.splash.transform, splashPosition, splashScale);
        UpdateTransform(spray.stream.transform, streamPosition, streamScale);

        spray.jet.GetComponent<SpriteRenderer>().sprite = jetSprite;

        var currentTargetPosition = gun.target.transform.position;
        var availablePosition = navigationMap.GetClosestPosition(currentTargetPosition);

        spray.transform.position += availablePosition - currentTargetPosition;

        patternDrawModificator = patternRelativeSize / -2;
    }

    public void OnSetActive(bool active, Gun gun, Vector3 position)
    {
        isMoving = active;

        if (active)
        {
            catchModificator = position - gun.target.transform.position;
            catchModificator.z = gun.target.transform.position.z;
            gun.TryDrawPixels(patternDrawModificator);
        }
    }

    public void OnMove(Vector3 position, Gun gun)
    {
        moveDestination = position - catchModificator;
        moveDestination.z = gun.target.transform.position.z;
    }

    public void OnDisable() 
    {
        isMoving = false;
    }

    public void OnFrameUpdate() 
    {
        if (!isMoving)
            return;
        
        var moveDirection = (moveDestination - gun.target.transform.position).normalized;
        var nextStep = gun.target.transform.position + moveDirection * moveStep;
        
        gun.transform.position += navigationMap.GetClosestPosition(nextStep) - gun.target.transform.position;
        gun.TryDrawPixels(patternDrawModificator);
    }

    public void SetSprite(Sprite gunModeSprite)
    {
        jetSprite = gunModeSprite;
    }

    private void UpdateTransform(Transform transform, Vector2 position, Vector2 scale)
    {
        transform.localPosition = new Vector3(position.x,
                                         position.y,
                                         transform.position.z);

        transform.localScale = new Vector3(scale.x,
                                           scale.y,
                                           transform.localScale.z);
    }
    
    private void LazyNavigationMapInit()
    {
        if (navigationMap == null)
        {
            var levelGenerator = GameObject.FindGameObjectWithTag("LevelGenerator");
            navigationMap = levelGenerator.GetComponent<LevelGenerator>().navigationMap;
        }
    }
}
