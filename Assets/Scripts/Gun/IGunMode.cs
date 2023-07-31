using UnityEngine;

public interface IGunMode
{
    public void SetSprite(Sprite gunModeSprite);

    public void OnEnable(Gun gun, Spray spray);
    public void OnDisable();
    public void OnSetActive(bool active, Gun gun, Vector3 position);
    public void OnMove(Vector3 position, Gun gun);
    public void OnFrameUpdate();
}