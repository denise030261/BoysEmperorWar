
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
    public Text CurrentPage;
    public TextMeshProUGUI MaxScore;
    public int UISpeed = 2; // UI 변경 속도
    public int isChange = 0; // UI 변경 여부 (0은 변경 없음, 1은 사라짐, 2는 채워짐)

    public Image[] StageBoard;
    private int[] StageMaxScore = new int[5];

    public GameObject[] StageLight; // 스테이지 불
    public bool[] IsEnter = new bool[5];

    public GameObject WarningDisplay;
    public GameObject OptionDisplay;
    public GameObject ManualDisplay;
    public GameObject MusicOptionDisplay;
    public GameObject StoryDisplay;
    public GameObject[] Manual;

    public Button[] StoryButtons;
    private bool PrePlay;
    public Image PrePlayImage;

    public int[] StandardScore = new int[5];

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

            if (i!=StageMaxScore.Length-1)
            {
                if (StageMaxScore[i] >= StandardScore[i])
                {
                    Debug.Log(i+"가 활성화");
                    StageBoard[i + 1].sprite = Resources.Load<Sprite>("UI/LevelArea");
                    StoryButtons[i+1].interactable = true;
                    StoryButtons[i+5].interactable = true;
                    IsEnter[i + 1] = true;
                } // 일정 점수를 넘겨야 풀리는 것이지만 아직 점수가 정해지지 않았으니 디폴트로 만점으로 처리한다.
                else if(StageMaxScore[i] < StandardScore[i] && i==0)
                {
                    StoryButtons[i + 1].interactable = false;
                    StoryButtons[i + 5].interactable = false;
                }
                else if (i != 0)
                {
                    StageBoard[i + 1].sprite = Resources.Load<Sprite>("UI/LevelLockArea");
                    StoryButtons[i + 1].interactable = false;
                    StoryButtons[i + 5].interactable = false;
                    IsEnter[i + 1] = false;
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

            Debug.Log((i + 1) + "MaxScore" + "에서의 최고 점수는 " + StageMaxScore[i]);
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
            Debug.Log("왼쪽으로 움직였습니다");
            PrePlay = false;
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
    } // UI 변경

    public void Onclick_Warning(bool On)
    {
        WarningDisplay.SetActive(On);
    } // 현재 스테이지 입장 불가 경고

    public void OnClick_OptionOn(bool On)
    {
        OptionDisplay.SetActive(On); 
    } // 옵션 화면 상태

    public void OnClick_ManualDisplay(bool On)
    {
        ManualDisplay.SetActive(On);
    } // 메뉴얼 화면 상태

    public void OnClick_GameEnd()
    {
        Application.Quit();
    } // 게임 끄기

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
    } // 메뉴얼 페이지 상태

    public void OnClick_MusicOption(bool On)
    {
        MusicOptionDisplay.SetActive(On);   
    } // 음향 옵션 화면 상태

    public void Onclick_Story(bool On)
    {
        StoryDisplay.SetActive(On);
    } // 스토리 화면 상태

    public void OnClick_StoryButton(int level)
    {
        PlayerPrefs.SetInt("StoryLevel", level);
        PlayerPrefs.SetString("Scene", "StageSelect");
        SceneManager.LoadScene("Loading(Imsi)");
    } // 스토리 상태

    public void OnClick_PlayMusic()
    {
        MainAudioManager.Instance.PlayMusicBGM(CurrentLevel.ToString());

        if(PrePlay)
        {
            Debug.Log("미리듣기 정지");
            PrePlay = false;
            MainAudioManager.Instance.StopMusicBGM();
            PrePlayImage.color = new Color(255, 255, 255);
        }
        else
        {
            Debug.Log("미리듣기 실행중");
            PrePlay = true;
            Color color1;
            ColorUtility.TryParseHtmlString("#959595", out color1);
            PrePlayImage.color = color1;
        }
    } // 미리듣기 
}
