using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class JetAnimation : MonoBehaviour
{
    public float animationTime = 0.05f;
    public Vector3 tremble = new(0.01f,0f,0f);
    public float alpha = 0.05f;
    
    private Color normalColor;
    private SpriteRenderer jetSprite;

    private void Start()
    {
        jetSprite = GetComponent<SpriteRenderer>();
        StartCoroutine(nameof(DrawAnimationFrame));
    }

    IEnumerator DrawAnimationFrame()
    {
        while(true) 
        {
            yield return new WaitForSeconds(animationTime);
            transform.position += tremble;
            jetSprite.color -= new Color(0f,0f,0f,alpha);
            alpha = -alpha;
            tremble = -tremble;
        }
    }
}
