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
        gun.SetActive(true, touchPositions.worldPosition);
    }

    private void OntouchMove(TouchPositions touchPositions)
    {
        if (gun.isActivated)
            gun.GunMoved(touchPositions.worldPosition);
    }

    private void OnTouchUp(TouchPositions touchPositions)
    {
        gun.SetActive(false, touchPositions.worldPosition);
    }
}
