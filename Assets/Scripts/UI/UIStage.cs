using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStage : MonoBehaviour
{
    public int CurrentLevel = 1;
    public int PreviousLevel = 1;

    public Image TitleUI;
    public Image StageUI;
    public Text CurrentPage;
    public int UISpeed = 2; // UI ���� �ӵ�
    public int isChange = 0; // UI ���� ���� (0�� ���� ����, 1�� �����, 2�� ä����)

    public Image[] StageBoard;
    private int[] StageMaxScore = new int[5];

    public GameObject[] StageLight; // �������� ��
    public bool[] IsEnter = new bool[5];

    public GameObject WarningDisplay;
    public GameObject OptionDisplay;
    public GameObject ManualDisplay;
    public GameObject[] Manual;

    public static UIStage Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", CurrentLevel);

        TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
        StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
        StageLight[CurrentLevel - 1].SetActive(true);
        IsEnter[0] = true;
    }

    private void Start()
    {
        for (int i = 0; i < StageMaxScore.Length - 1; i++)
        {
            StageMaxScore[i] = PlayerPrefs.GetInt((i + 1) + "MaxScore", 0);

            if (StageMaxScore[i] >= 10000)
            {
                StageBoard[i + 1].sprite = Resources.Load<Sprite>("UI/LevelArea");
                IsEnter[i+1] = true;
            } // ���� ������ �Ѱܾ� Ǯ���� �������� ���� ������ �������� �ʾ����� ����Ʈ�� �������� ó���Ѵ�.
            else if (i != 0)
            {
                StageBoard[i+1].sprite = Resources.Load<Sprite>("UI/LevelLockArea");
                IsEnter[i + 1] = false;
            }
            Debug.Log((i + 1) + "MaxScore" + "������ �ְ� ������ " + StageMaxScore[i]);
        }
    }

    void Update()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
        UIChange();
    }
    private void UIChange()
    {
        if (isChange == 1)
        {
            if (TitleUI.rectTransform.anchoredPosition.x >= -357)
            {
                float UITitleRecX = TitleUI.rectTransform.anchoredPosition.x + Time.deltaTime * 5000;
                TitleUI.rectTransform.anchoredPosition = new Vector2(UITitleRecX, TitleUI.rectTransform.anchoredPosition.y);

                float UIStageRecX = StageUI.rectTransform.anchoredPosition.x - Time.deltaTime * 4000;
                StageUI.rectTransform.anchoredPosition = new Vector2(UIStageRecX, StageUI.rectTransform.anchoredPosition.y);
            }
        }
        else if (isChange == 2)
        {
            if (TitleUI.rectTransform.anchoredPosition.x >= 357)
            {
                TitleUI.rectTransform.anchoredPosition = new Vector2(357, TitleUI.rectTransform.anchoredPosition.y);
                StageUI.rectTransform.anchoredPosition = new Vector2(-370, StageUI.rectTransform.anchoredPosition.y);
            }
            if (TitleUI.rectTransform.anchoredPosition.x <= 357)
            {
                float UITitleRecX = TitleUI.rectTransform.anchoredPosition.x - Time.deltaTime * 5000;
                TitleUI.rectTransform.anchoredPosition = new Vector2(UITitleRecX, TitleUI.rectTransform.anchoredPosition.y);

                float UIStageRecX = StageUI.rectTransform.anchoredPosition.x + Time.deltaTime * 4000;
                StageUI.rectTransform.anchoredPosition = new Vector2(UIStageRecX, StageUI.rectTransform.anchoredPosition.y);

            }

            if (TitleUI.rectTransform.anchoredPosition.x <= -357)
            {
                isChange = 0;
                TitleUI.rectTransform.anchoredPosition = new Vector2(-357, TitleUI.rectTransform.anchoredPosition.y);
                StageUI.rectTransform.anchoredPosition = new Vector2(370, StageUI.rectTransform.anchoredPosition.y);
                StageLight[CurrentLevel - 1].SetActive(true); // �������� �� Ű��
            }
        }
    } // UI ����

    public void Onclick_Warning(bool On)
    {
        WarningDisplay.SetActive(On);
    } // ���� �������� ���� �Ұ� ���

    public void OnClick_OptionOn(bool On)
    {
        OptionDisplay.SetActive(On); 
    }

    public void OnClick_ManualDisplay(bool On)
    {
        ManualDisplay.SetActive(On);
    }

    public void OnClick_GameEnd()
    {
        Application.Quit();
    }

    public void OnClick_ManulPage(int Page)
    {
        CurrentPage.text = Page.ToString();
        if (Page == 1)
        {
            Manual[Page - 1].SetActive(true);
            Manual[Page].SetActive(false);
        }
        else if(Page==2)
        {
            Manual[Page - 1].SetActive(true);
            Manual[Page-2].SetActive(false);
        }
    }
}