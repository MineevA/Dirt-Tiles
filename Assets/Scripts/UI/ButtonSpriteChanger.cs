using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonSpriteChanger : MonoBehaviour
{

    public Sprite firstImageTexture;
    public Sprite secondImageTexture;

    private Image image;
    private bool firstImage;

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = firstImageTexture;
        firstImage = true;
    }

    public void OnButtonClick()
    {
        firstImage = !firstImage;

        if (firstImage)
            image.sprite = firstImageTexture;
        else
            image.sprite = secondImageTexture;
    }

}
