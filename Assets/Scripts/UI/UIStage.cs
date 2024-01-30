
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIStage : MonoBehaviour
{
    public int CurrentLevel = 1;
    public int PreviousLevel = 1;

    public Image TitleUI;
    public Image StageUI;
    public TextMeshProUGUI MaxScore;
    public int UISpeed = 2; // UI ���� �ӵ�
    public int isChange = 0; // UI ���� ���� (0�� ���� ����, 1�� �����, 2�� ä����)

    public Image[] StageBoard;
    private int[] StageMaxScore = new int[5];

    public GameObject[] StageLight; // �������� ��
    public bool[] IsEnter = new bool[5];

    public Button[] StoryButtons;

    public int[] StandardScore = new int[5];

    public bool AllClearMode = false;

    public static UIStage Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", CurrentLevel);

        TitleUI.sprite = Resources.Load<Sprite>("UI/SongTitle0" + CurrentLevel);
        StageUI.sprite = Resources.Load<Sprite>("UI/Stage0" + CurrentLevel);
        IsEnter[0] = true;
        StoryButtons[0].interactable = true;
    }

    private void Start()
    {
        for (int i = 0; i < StageMaxScore.Length; i++)
        {
            StageMaxScore[i] = PlayerPrefs.GetInt((i + 1) + "MaxScore", 0);

            if(AllClearMode)
            {
                StoryButtons[i].interactable = true;
                StoryButtons[i + 5].interactable = true;
                IsEnter[i] = true;
                StageBoard[i].sprite = Resources.Load<Sprite>("UI/LevelArea");
            }
            else
            {
                if (i != StageMaxScore.Length - 1)
                {
                    if (StageMaxScore[i] >= StandardScore[i])
                    {
                        Debug.Log(i + "�� Ȱ��ȭ");
                        StageBoard[i + 1].sprite = Resources.Load<Sprite>("UI/LevelArea");
                        StoryButtons[i + 1].interactable = true;
                        StoryButtons[i + 5].interactable = true;
                        IsEnter[i + 1] = true;
                    } // ���� ������ �Ѱܾ� Ǯ���� ��
                    else
                    {
                        if(i!=0)
                        {
                            StageBoard[i + 1].sprite = Resources.Load<Sprite>("UI/LevelLockArea");
                            IsEnter[i + 1] = false;
                        }
                        StoryButtons[i + 1].interactable = false;
                        StoryButtons[i + 5].interactable = false;
                    }
                }
                else
                {
                    if (StageMaxScore[i] >= StandardScore[i])
                    {
                        StoryButtons[i + 5].interactable = true;
                    }
                    else
                    {
                        StoryButtons[i + 5].interactable = false;
                    }
                }
            }

            Debug.Log((i + 1) + "MaxScore" + "������ �ְ� ������ " + StageMaxScore[i]);
        }


        if (StageBoard[CurrentLevel - 1].sprite.name == "LevelArea")
        {
            StageLight[CurrentLevel - 1].SetActive(true);
        }

        MaxScore.text = StageMaxScore[CurrentLevel - 1].ToString();
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
            Debug.Log("�������� ���������ϴ�");
            UIOption.Instance.PrePlay = false;
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
                StageUI.rectTransform.anchoredPosition = new Vector2(-357, StageUI.rectTransform.anchoredPosition.y);
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
                StageUI.rectTransform.anchoredPosition = new Vector2(357, StageUI.rectTransform.anchoredPosition.y);
                if (StageBoard[CurrentLevel - 1].sprite.name == "LevelArea")
                {
                    StageLight[CurrentLevel - 1].SetActive(true);
                    MaxScore.text = StageMaxScore[CurrentLevel - 1].ToString();
                }
            }
        }
    } // UI ����

    public void OnClick_StoryButton(int level)
    {
        MainAudioManager.Instance.StopMusicBGM();
        PlayerPrefs.SetInt("StoryLevel", level);
        PlayerPrefs.SetString("Scene", "StageSelect");
        SceneManager.LoadScene("Loading(Imsi)");
    } // ���丮 ����
}
