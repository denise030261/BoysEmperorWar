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
        CurrentStage = PlayerPrefs.GetInt("Level", 1);
        CurrentState = PlayerPrefs.GetString("State", "Before");
        LoadJsonFile();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        MainAudioManager.Instance.PlayBGM("현재");
        // 게임 전후 구분 Prefs 필요
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && IsProgress && !IsChat)
        {
            if(PlaceData.Count==SceneNum+1)
            {
                MainAudioManager.Instance.StopBGM();

                if (CurrentState == "Before")
                    SceneManager.LoadScene("Game");
                else
                    SceneManager.LoadScene("StageSelect");
            }

            if (DataManager.Instance.SceneNum + 1 <= DataManager.Instance.PlaceData.Count)
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

            if (BGMData[SceneNum] != "")
            {
                MainAudioManager.Instance.PlayBGM("급박");
            }
        }
    }

    private void LoadJsonFile()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/" + CurrentState + "Ep" + CurrentStage);
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
    public string Chat;
    public string ChatWindow;
    public string Place;
    public string BGM;
}
