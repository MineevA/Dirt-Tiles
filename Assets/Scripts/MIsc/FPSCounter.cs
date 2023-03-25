using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text text;
    private float timer;
    private int fps;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        fps++;
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            text.text = fps.ToString();
            fps = 0;
            timer = 0f;
        }
    }
}
