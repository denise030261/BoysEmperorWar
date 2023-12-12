using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public struct ScoreData
{
    public int perfect;
    public int great;
    public int good;
    public int miss;

    public string[] judgeText;
    public Color[] judgeColor;
    public JudgeType judge;
    public int combo;
    public int maxcombo;

    public int score 
    { 
        get
        {
            return (perfect*1000)+(great * 500) + (good * 200);
        }
        set
        {
            score = value;
        }
    }
}

public class Score : MonoBehaviour
{
    static Score instance;
    public static Score Instance
    {
        get { return instance; }
    }

    public ScoreData data;

    UIImage uiJudgement;
    UIImage uiStage;
    UIText uiCombo;
    UIText uiScore;

    private Sprite uiStageSprite;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Init()
    {
        uiStage = UIController.Instance.FindUI("UI_I_Stage").uiObject as UIImage;
        uiJudgement = UIController.Instance.FindUI("UI_G_Judgement").uiObject as UIImage;
        uiCombo = UIController.Instance.FindUI("UI_G_Combo").uiObject as UIText;
        uiScore = UIController.Instance.FindUI("UI_G_Score").uiObject as UIText;

        AniPreset.Instance.Join(uiStage.Name);
        AniPreset.Instance.Join(uiJudgement.Name);
        AniPreset.Instance.Join(uiCombo.Name);
        AniPreset.Instance.Join(uiScore.Name);
    }

    public void Clear()
    {
        //Sprite uiStageSprite = Resources.Load<Sprite>("Play/SmallST" + GameManager.Instance.CurrentStage);
        string imagePath = Path.Combine(Application.streamingAssetsPath, "Play/SmallST" + GameManager.Instance.CurrentStage+".png");
        StartCoroutine(LoadImage(imagePath));

        data = new ScoreData();
        data.judgeText = Enum.GetNames(typeof(JudgeType));
        //uiStage.SetSprite(uiStageSprite);
        uiJudgement.SetSprite(null);
        uiJudgement.SetAlphaToOne(0);
        uiCombo.SetText("");
        uiScore.SetText("0");
    }

    public void SetScore()
    {
        Sprite uiJudgementSprite = Resources.Load<Sprite>("Play/" + data.judgeText[(int)data.judge]);
        uiJudgement.SetSprite(uiJudgementSprite);
        uiJudgement.SetAlphaToOne(1);
        uiCombo.SetText($"{data.combo}");
        uiScore.SetText($"{data.score}");

        if(data.maxcombo<data.combo)
        {
            data.maxcombo = data.combo; 
        }

        AniPreset.Instance.PlayPop(uiJudgement.Name, uiJudgement.rect);
        AniPreset.Instance.PlayPop(uiCombo.Name, uiCombo.rect);
        UIController.Instance.find.Invoke(uiJudgement.Name);
        UIController.Instance.find.Invoke(uiCombo.Name);
    }

    public void Ani(UIObject uiObject)
    {
       // AniPreset.Instance.PlayPop(uiObject.Name, uiObject.rect);
    }

    IEnumerator LoadImage(string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            // 이제 texture를 BackGroundImage.sprite에 할당하면 됩니다.
            uiStageSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            uiStage.SetSprite(uiStageSprite);
        }
        else
        {
            Debug.LogError("Failed to load image: " + www.error + " Path : " + path);
        }
    }
}