using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public float FadeDuration = 3.0f;
    protected float StartTime;
    public Image FadeImage;
    public GameObject FadeInObject;

    // Start is called before the first frame update
    void Start()
    {
        FadeInInit();
    }

    // Update is called once per frame
    void Update()
    {
        FadeIn();
    }

    protected void FadeIn()
    {
        float elapsedTime = Time.time - StartTime;

        float alpha = 1 - Mathf.Clamp01(elapsedTime / FadeDuration);

        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);

        if (alpha <= 0)
        {
            FadeInObject.SetActive(false);
        }
    }

    protected void FadeInInit()
    {
        FadeInObject.SetActive(true);
        StartTime = Time.time;
    }
}
