using UnityEngine;

[RequireComponent(typeof(Gun))]
public class GunInputController : MonoBehaviour
{
    private InputManager inputManager;
    private Gun gun;
    
    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        inputManager.OnTouchDown += OnTouchDown;
        inputManager.OnTouchMove += OntouchMove;
        inputManager.OnTouchUp += OnTouchUp;

        gun = GetComponent<Gun>();
    }

    private void OnTouchDown(TouchPositions touchPositions)
    {
        gun.IsActivated = true;
        transform.position = touchPositions.worldPosition;
    }

    private void OntouchMove(TouchPositions touchPositions)
    {
        if (gun.IsActivated)
        {
            transform.position = touchPositions.worldPosition;
            gun.GunMoved();
        }
    }

    private void OnTouchUp(TouchPositions touchPositions)
    {
        gun.IsActivated = false;
    }
}
