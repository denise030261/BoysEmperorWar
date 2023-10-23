using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlaceManager : UIFade
{
    public int[] Before1Place;
    public int[] After1Place;

    public Image StoryBackGround; // 배경화면
    public float DelayTime;

    private bool IsFade=false;

    // Start is called before the first frame update
    void Start()
    {
        FadeInInit();
        StartCoroutine(DelayTitle());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFade && FadeImage.rectTransform.anchoredPosition.x>=-800)
        {
            FadeIn();
            MovePlaceTitle();
        }
    }

    IEnumerator DelayTitle()
    {
        yield return new WaitForSeconds(DelayTime);
        StartTime = Time.time;
        IsFade = true;
    }

    void MovePlaceTitle()
    {
        float UITitleRecX = FadeImage.rectTransform.anchoredPosition.x - Time.deltaTime * 500;
        FadeImage.rectTransform.anchoredPosition = new Vector2(UITitleRecX, FadeImage.rectTransform.anchoredPosition.y);
    }
}
// 처음에는 뜨고 엔터 누르면 사라지기