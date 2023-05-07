using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform gunTarget;
    public InputManager inputManager;

    [Range(0f, 1f)]
    public float velocityModifier;

    [Range(0f, 10f)]
    public float brakeIntensity;

    public Bounds cameraBounds;

    private Vector3 velocity;
    private float brake;
        
    // Start is called before the first frame update
    void Start()
    {
        SetDefaultControls();
    }
    
    public void SetDefaultControls()
    {
        SetControls(true, true, true);
    }

    public void SetControls(bool onTouchDown, bool onTouchMove, bool onTouchUp)
    {
        DropControls();
        
        if (onTouchDown)
            inputManager.OnTouchDown += OnTouchDown;

        if (onTouchMove)
            inputManager.OnTouchMove += OnTouchMove;

        if (onTouchUp)
            inputManager.OnTouchUp += OnTouchUp;
    }

    public void DropControls()
    {
        inputManager.OnTouchDown -= OnTouchDown;
        inputManager.OnTouchMove -= OnTouchMove;
        inputManager.OnTouchUp -= OnTouchUp;
    }

    private void OnTouchDown(TouchPositions touchPositions)
    {
        brake = 0f;
    }

    private void OnTouchMove(TouchPositions touchPositions)
    {
        SetVelocityByTouch(touchPositions.worldPosition);
    }

    private void OnTouchUp(TouchPositions touchPositions)
    {
        brake = brakeIntensity;
    }

    private void SetVelocityByTouch(Vector3 touchPosition)
    {
        Vector2 gunDestination = gunTarget.position - transform.position;
        Vector2 touchDestination = touchPosition - transform.position;

        if (gunDestination.magnitude >= touchDestination.magnitude)
            SetVelocity(gunDestination);
        else
            SetVelocity(touchDestination);
    }

    private void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity * velocityModifier;
        this.velocity.z = 0f;
    }
 
    private void MoveCamera()
    {
        var nextPosition = transform.position + velocity * Time.deltaTime;
        if (cameraBounds.Contains(nextPosition))
            transform.position = nextPosition;
        else
            transform.position = cameraBounds.ClosestPoint(nextPosition);

        velocity -= brake * Time.deltaTime * velocity.normalized;
    }

    void Update()
    {
        MoveCamera();
    }
}
