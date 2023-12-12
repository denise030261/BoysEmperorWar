using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    public Text CurrentPage;
    public int CurrentLevel = 1;

    public GameObject WarningDisplay;
    public GameObject OptionDisplay;
    public GameObject ManualDisplay;
    public GameObject MusicOptionDisplay;
    public GameObject ScreenOptionDisplay;
    public GameObject StoryDisplay;
    public GameObject[] Manual;

    public bool PrePlay;
    public Image PrePlayImage;

    public static UIOption Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
        PlayerPrefs.SetInt("Level", CurrentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    public void Onclick_Warning(bool On)
    {
        WarningDisplay.SetActive(On);
    } // ���� �������� ���� �Ұ� ���

    public void OnClick_OptionOn(bool On)
    {
        OptionDisplay.SetActive(On);
    } // �ɼ� ȭ�� ����

    public void OnClick_ManualDisplay(bool On)
    {
        ManualDisplay.SetActive(On);
    } // �޴��� ȭ�� ����

    public void OnClick_GameEnd()
    {
        Application.Quit();
    } // ���� ����

    public void OnClick_ManulPage(int Page)
    {
        CurrentPage.text = Page.ToString();
        if (Page == 1)
        {
            Manual[Page - 1].SetActive(true);
            Manual[Page].SetActive(false);
        }
        else if (Page == 2)
        {
            Manual[Page - 1].SetActive(true);
            Manual[Page - 2].SetActive(false);
        }
    } // �޴��� ������ ����

    public void OnClick_MusicOption(bool On)
    {
        MusicOptionDisplay.SetActive(On);
    } // ���� �ɼ� ȭ�� ����

    public void Onclick_Story(bool On)
    {
        StoryDisplay.SetActive(On);
    } // ���丮 ȭ�� ����

    public void Onclick_ScreenOption(bool On)
    {
        ScreenOptionDisplay.SetActive(On);
    } // ���丮 ȭ�� ����

    public void OnClick_PlayMusic()
    {
        MainAudioManager.Instance.PlayMusicBGM(CurrentLevel.ToString());

        if (PrePlay)
        {
            Debug.Log("�̸���� ����");
            PrePlay = false;
            MainAudioManager.Instance.StopMusicBGM();
            PrePlayImage.color = new Color(255, 255, 255);
        }
        else
        {
            Debug.Log("�̸���� ������");
            PrePlay = true;
            Color color1;
            ColorUtility.TryParseHtmlString("#959595", out color1);
            PrePlayImage.color = color1;
        }
    } // �̸���� 
}
