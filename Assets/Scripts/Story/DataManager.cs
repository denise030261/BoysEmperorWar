using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    protected List<string> LeftCharacterData = new List<string>();
    protected List<string> RightCharacterData = new List<string>();
    protected List<string> ChatData = new List<string>();
    protected List<string> ChatWindowData = new List<string>();
    protected List<string> PlaceData = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        LoadJsonFile();
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
