using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] spritesToPlay;
    public bool loop = true;
    public bool random = true;
    public bool playOnAwake = true;
    public float cycleTime = 1f;

    private bool playing = false;
    private float frameTime;
    private int currentFrame;
    private SpriteRenderer spriteRenderer;
    
    public bool isPlaying()
    {
        return playing;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (playOnAwake)
            StartAnimation();
    }

    private void OnEnable()
    {
        if (spriteRenderer != null)
            StartAnimation();
    }

    public void PlaySprites(ref Sprite[] spritesToLoad)
    {
        StopAnimation();
        spritesToPlay = spritesToLoad;
        StartAnimation();
    }

    public void LoadSpritesOnPlay(ref Sprite[] spritesToLoad)
    {
        spritesToPlay = spritesToLoad;
        currentFrame = 0;
        frameTime = cycleTime / spritesToPlay.Length;
    }

    private void StartAnimation()
    {
        if (spritesToPlay?.Length > 0)
        {
            frameTime = cycleTime / spritesToPlay.Length;
            currentFrame = 0;
            playing = true;
            StartCoroutine(nameof(AnimationPlay));
        }
    }

    private void StopAnimation()
    {
        playing = false;
        StopAllCoroutines();
    }

    IEnumerator AnimationPlay()
    {
        while (loop || currentFrame < spritesToPlay.Length)
        {
            if (random)
                spriteRenderer.sprite = spritesToPlay[Random.Range(0, spritesToPlay.Length - 1)];
            else
                spriteRenderer.sprite = spritesToPlay[currentFrame];

            currentFrame++;
            if (currentFrame == spritesToPlay.Length && loop)
                currentFrame = 0;
            
            yield return new WaitForSeconds(frameTime);
        }
    }
}
