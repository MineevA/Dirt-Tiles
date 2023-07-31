using UnityEngine;

[RequireComponent(typeof(Gun))]
public class GunInputController : MonoBehaviour
{
    private InputManager inputManager;
    private Gun gun;
    private readonly string gunTag = "Gun";
     
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
        var ray = new Ray(touchPositions.worldPosition, Vector3.forward);

        if (Physics.Raycast(ray, out var hit,2f, LayerMask.GetMask(gunTag)))
            gun.SetActive(true, touchPositions.worldPosition);
    }

    private void OntouchMove(TouchPositions touchPositions)
    {
        if (gun.inControl)
            gun.GunMoved(touchPositions.worldPosition);
    }

    private void OnTouchUp(TouchPositions touchPositions)
    {
        if (gun.inControl)
            gun.SetActive(false, touchPositions.worldPosition);
    }
}
