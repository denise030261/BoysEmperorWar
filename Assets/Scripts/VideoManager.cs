using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; } = null;

    private int ResolutionHeight;
    private int ResolutionWidth;
    private bool ResolutionFullScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        int IsFullScreen = PlayerPrefs.GetInt("ResolutionFullScreen", 1);
        PlayerPrefs.SetInt("ResolutionFullScreen", IsFullScreen);
        ResolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        PlayerPrefs.SetInt("ResolutionHeight", ResolutionHeight);
        ResolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        PlayerPrefs.SetInt("ResolutionWidth", ResolutionWidth);

        if (IsFullScreen == 1)
            ResolutionFullScreen = true;
        else if (IsFullScreen == 0)
            ResolutionFullScreen = false;
    }

    public void SetResolution()
    {
        Debug.Log(ResolutionWidth + " " + ResolutionHeight + " " + ResolutionFullScreen);
        Screen.SetResolution(ResolutionWidth, ResolutionHeight, ResolutionFullScreen);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetResolution();
    }
}
