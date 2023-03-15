using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public delegate void OnTouchAction(TouchPositions touchPositions);

    public event OnTouchAction OnTouchDown;
    public event OnTouchAction OnTouchMove;
    public event OnTouchAction OnTouchUp;

    private TouchControls touchControls;

    private void Awake()
    {
        touchControls = new TouchControls();   
    }

    private void OnEnable()
    {
        touchControls.Enable();  
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        touchControls.TouchMap.TouchPress.started += OnTouchStart;
        touchControls.TouchMap.TouchPress.canceled += OnTouchEnded;
        touchControls.TouchMap.TouchMoved.started += OnTouchMoved;
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        if (OnTouchDown != null && !IsPointerOverUI(EventSystem.current))
            OnTouchDown.Invoke(new TouchPositions(TouchScreenPosition()));
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        OnTouchUp?.Invoke(new TouchPositions(TouchScreenPosition()));
    }

    private void OnTouchMoved(InputAction.CallbackContext context)
    {
        if (OnTouchMove != null && !IsPointerOverUI(EventSystem.current)) 
            OnTouchMove.Invoke(new TouchPositions(TouchScreenPosition()));
    }

    private Vector2 TouchScreenPosition()
    {
        return touchControls.TouchMap.TouchPosition.ReadValue<Vector2>();
    }

    private bool IsPointerOverUI(EventSystem eventSystem)
    {
        var pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touchControls.TouchMap.TouchPosition.ReadValue<Vector2>();

        var results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);

        return results.Count > 0;
    }
}

public readonly struct TouchPositions
{
    public readonly Vector2 screenPosition;
    public readonly Vector3 worldPosition;

    public TouchPositions(Vector2 touchScreenPosition)
    {
        var camera = Camera.main;
        var viewport = new Vector3(touchScreenPosition.x,
                                   touchScreenPosition.y,
                                   0f - camera.transform.position.z);
        worldPosition = camera.ScreenToWorldPoint(viewport);
        screenPosition = touchScreenPosition;
    }
}
