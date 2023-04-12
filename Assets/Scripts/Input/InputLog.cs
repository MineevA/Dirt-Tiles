using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class InputLog : MonoBehaviour
{
    
    private InputManager inputManager;
    
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.OnTouchDown += LogEventDown;
        inputManager.OnTouchUp += LogEventUp;
        inputManager.OnTouchMove += LogEventMove;
    }

    void LogEventDown(TouchPositions touchPositions)
    {
        LogEvent("On touch down", touchPositions);
    }

    void LogEventUp(TouchPositions touchPositions)
    {
        LogEvent("On touch up", touchPositions);
    }

    void LogEventMove(TouchPositions touchPositions)
    {
        LogEvent("On touch move", touchPositions);
    }

    void LogEvent(string logText, TouchPositions touchPositions)
    {
        Debug.Log(logText);
        Debug.Log("    world " + touchPositions.worldPosition);
        Debug.Log("    screen " + touchPositions.screenPosition);
    }
}
