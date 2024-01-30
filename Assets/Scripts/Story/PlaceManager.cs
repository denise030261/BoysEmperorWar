using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceManager : UIFade
{
    private Image StoryBackGround; // 배경화면
    public float DelayTime;
    public Text PlaceName;

    private int CurrentStage;
    private string CurrentState;

    private bool IsFade=false;

    void Start()
    {
        StoryBackGround=GetComponent<Image>();
        PlaceName.text = DataManager.Instance.PlaceData[DataManager.Instance.SceneNum];
        FadeInInit();
        StartCoroutine(DelayTitle());
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.SceneNum + 1 <= DataManager.Instance.PlaceData.Count)
        {
            if (!DataManager.Instance.IsChat && DataManager.Instance.IsProgress &&
            Input.GetKeyUp(KeyCode.Return))
            {
                if (DataManager.Instance.SceneNum + 1 == DataManager.Instance.PlaceData.Count)
                    return;
                else if(DataManager.Instance.PlaceData[DataManager.Instance.SceneNum + 1] != "")
                {
                    PlaceName.text = DataManager.Instance.PlaceData[DataManager.Instance.SceneNum + 1];
                    FadeInInit();
                    StartCoroutine(DelayTitle());
                }
            }// index 넘는거 조심

            if (IsFade && FadeImage.rectTransform.anchoredPosition.x >= -800)
            {
                FadeIn();
                MovePlaceTitle();
            }
            else if (FadeImage.rectTransform.anchoredPosition.x < -800)
            {
                IsFade = false;
            }// 장소 이미지 넘기기
        }
    }

    IEnumerator DelayTitle()
    {
        CurrentStage = PlayerPrefs.GetInt("Level", 1);
        CurrentState = PlayerPrefs.GetString("State", "Before");

        FadeImage.rectTransform.anchoredPosition = new Vector2(0, FadeImage.rectTransform.anchoredPosition.y);
        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 1);

        if (DataManager.Instance.SceneNum == 0)
        {
            StoryBackGround.sprite = Resources.Load<Sprite>
           ("Story/Place/" + DataManager.Instance.PlaceData[DataManager.Instance.SceneNum]);
        }
        else
        {
            StoryBackGround.sprite = Resources.Load<Sprite>
          ("Story/Place/"+ DataManager.Instance.PlaceData[DataManager.Instance.SceneNum+1]);
        }

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