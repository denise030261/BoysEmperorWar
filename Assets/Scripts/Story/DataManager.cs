using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public int CurrentStage;
    public string CurrentState;

    protected List<string> LeftCharacterData = new List<string>();
    protected List<string> RightCharacterData = new List<string>();
    protected List<string> CenterCharacterData = new List<string>();
    protected List<string> CenterCharacterTwoData = new List<string>();
    protected List<string> ChatData = new List<string>();
    protected List<string> ChatWindowData = new List<string>();
    public List<string> PlaceData = new List<string>();
    public List<string> BGMData = new List<string>();

    public int SceneNum = 0;
    public bool IsProgress=false;
    public bool IsChat = false;
    public bool IsPlaceWindow = false;   

    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CurrentStage= PlayerPrefs.GetInt("StoryLevel", 0);
        if (CurrentStage > 0 && CurrentStage<6)
        {
            Debug.Log("이전 스토리 모드 진입" + CurrentStage);
            PlayerPrefs.SetString("State", "Before");
        }
        else if(CurrentStage>5)
        {
            CurrentStage -= 5;
            Debug.Log("이후 스토리 모드 진입" + CurrentStage);
            PlayerPrefs.SetString("State", "After");
        }
        else
        {
            CurrentStage = PlayerPrefs.GetInt("Level", 1);
        }

        CurrentState= PlayerPrefs.GetString("State","Before");

        LoadJsonFile(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        //MainAudioManager.Instance.PlayBGM("현재");
        MainAudioManager.Instance.StopBGM();
        // StopBGM은 지우고 첫 번째 브금으로 적용

        Debug.Log(CurrentStage + " " + CurrentState);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && IsProgress && !IsChat)
        {
            if(PlaceData.Count==SceneNum+1)
            {
                MainAudioManager.Instance.StopBGM();

                int StoryLevel = PlayerPrefs.GetInt("StoryLevel", 0);
                if (CurrentState == "Before" && StoryLevel==0)
                    SceneManager.LoadScene("Game");
                else
                    SceneManager.LoadScene("StageSelect");
            }

            if (SceneNum + 1 <= PlaceData.Count)
            {
                if (SceneNum + 1 == PlaceData.Count)
                {
                    return;
                }
                else
                {
                    if (PlaceData[SceneNum + 1] != "")
                    {
                        SceneNum++;
                        IsChat = false;
                        IsProgress = false;
                    }
                    else
                    {
                        SceneNum++;
                        IsChat = true;
                        IsProgress = false;
                    }
                }
            }

           if(BGMData[SceneNum] != "")
            {
                MainAudioManager.Instance.PlayBGM(BGMData[SceneNum]);
            }
        }
    }

    private void LoadJsonFile()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/" + CurrentState + "Ep" + CurrentStage);
        Debug.Log("Json 파일은 " + "Json/" + CurrentState + "Ep" + CurrentStage);
        if (jsonFile != null)
        {
            string jsonData = jsonFile.text;
            StorySet StoryDataList = JsonUtility.FromJson<StorySet>(jsonData);

            Debug.Log(StoryDataList.Story.Count);
            if (StoryDataList != null && StoryDataList.Story.Count > 0)
            {
                foreach (StoryDatas StoryData in StoryDataList.Story)
                {
                    LeftCharacterData.Add(StoryData.LeftCharacter);
                    RightCharacterData.Add(StoryData.RightCharacter);
                    CenterCharacterData.Add(StoryData.CenterCharacter);
                    CenterCharacterTwoData.Add(StoryData.CenterCharacterTwo);
                    ChatData.Add(StoryData.Chat);
                    ChatWindowData.Add(StoryData.ChatWindow);
                    PlaceData.Add(StoryData.Place);
                    BGMData.Add(StoryData.BGM);
                }
            }
            else
            {
                Debug.LogError("Invalid JSON data or empty sets in the JSON file.");
            }
        }
        else
        {
            Debug.LogError("JSON file not found.");
        }
    }

    public void OnClick_Back()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void OnClick_Skip()
    {
        MainAudioManager.Instance.StopBGM();
        int StoryLevel = PlayerPrefs.GetInt("StoryLevel", 0);
        if (CurrentState=="Before" && StoryLevel==0)
            SceneManager.LoadScene("Game");
        else
            SceneManager.LoadScene("StageSelect");
    }
}

[System.Serializable]
public class StorySet
{
    public List<StoryDatas> Story;
}

[System.Serializable]
public class StoryDatas
{
    public string LeftCharacter;
    public string RightCharacter;
    public string CenterCharacter;
    public string CenterCharacterTwo;
    public string Chat;
    public string ChatWindow;
    public string Place;
    public string BGM;
}
