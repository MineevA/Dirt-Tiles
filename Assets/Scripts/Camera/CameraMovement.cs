using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform gunTarget;
    public InputManager inputManager;

    [Range(0f, 1f)]
    public float velocityModifier;

    [Range(0f, 10f)]
    public float velocitySquareDistanceModifier;
    
    [Range(0f, 10f)]
    public float returnSpeed;

    [Range(0f, 1f)]
    public float trembleReduce;

    public Bounds cameraBounds;

    private Vector3 velocity;
    private Vector3 startPosition;
    
    private bool returnMode;
        
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
       
        inputManager.OnTouchDown += OnTouchDown;
        inputManager.OnTouchMove += OnTouchMove;
        inputManager.OnTouchUp += OnTouchUp;
    }

    private void OnTouchDown(TouchPositions touchPositions)
    {
        SetVelocityByTouch(touchPositions.worldPosition);
    }

    private void OnTouchMove(TouchPositions touchPositions)
    {
        SetVelocityByTouch(touchPositions.worldPosition);
    }

    private void SetVelocityByTouch(Vector3 touchPosition)
    {
        Vector2 gunDestination = gunTarget.position - transform.position;
        Vector2 touchDestination = touchPosition - transform.position;

        if (gunDestination.magnitude >= touchDestination.magnitude)
            SetVelocityByDestination(gunTarget.position);
        else
            SetVelocityByDestination(touchPosition);
    }

    private void OnTouchUp(TouchPositions touchPositions)
    {
        returnMode = true;
    }

    private void SetVelocityByDestination(Vector3 destination)
    {
        returnMode = false;
        SetVelocity((destination - transform.position) * velocityModifier, velocitySquareDistanceModifier);
    }

    private void SetVelocity(Vector3 velocity, float squareModifier)
    {
        this.velocity = velocity.normalized * velocity.magnitude * velocity.magnitude * squareModifier;
        this.velocity.z = 0f;
    }
    
    private void MoveCamera()
    {
        var nextPosition = transform.position + velocity * Time.deltaTime;
        if (cameraBounds.Contains(nextPosition))
            transform.position = nextPosition;
        else
            transform.position = cameraBounds.ClosestPoint(nextPosition);
    }

    void Update()
    {
        MoveCamera();

        if (returnMode)
        {
            var distance = startPosition - transform.position;
            if (distance.magnitude < trembleReduce)
            {
                returnMode = false;
                distance = Vector3.zero;
            }
                
            SetVelocity(distance.normalized * returnSpeed, 1f);
        }
    }
}
