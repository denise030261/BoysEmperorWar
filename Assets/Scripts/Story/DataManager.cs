using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int CurrentStage;

    protected List<string> LeftCharacterData = new List<string>();
    protected List<string> RightCharacterData = new List<string>();
    protected List<string> ChatData = new List<string>();
    protected List<string> ChatWindowData = new List<string>();
    public List<string> PlaceData = new List<string>();

    public int SceneNum = 0;
    public bool IsProgress=false;
    public bool IsChat = false;
    public bool IsPlaceWindow = false;   

    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        LoadJsonFile();
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentStage = PlayerPrefs.GetInt("Level", 1);
        // 게임 전후 구분 Prefs 필요
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && IsProgress && !IsChat)
        {
            if(PlaceData[SceneNum + 1] != "")
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

    private void LoadJsonFile()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/BeforeEp1");
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
}
