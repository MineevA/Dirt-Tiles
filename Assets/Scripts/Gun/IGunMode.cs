using UnityEngine;

public interface IGunMode
{
    public void SetSprite(Sprite gunModeSprite);
    public void OnEnable(Spray spray);
    public void OnDisable(Spray spray);
    public void OnCleanStart(Spray spray);
    public void OnCleanNext(Spray spray);
    public void OnFrameUpdate(Spray spray);
}