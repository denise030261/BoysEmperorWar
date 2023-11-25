using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.TextCore;
using UnityEngine.UI;
using Screen = UnityEngine.Screen;

public class VideoOption : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutionArray=new Resolution[3];

    void Start()
    {
        InitializeResolutionOptions();
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
    }

    void InitializeResolutionOptions()
    {
        resolutionDropdown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;
        int resolutionIndex = 0;

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            if(Mathf.Approximately((float)resolution.width / resolution.height, 16f / 9f) && resolution.refreshRate == 60)
            {
                resolutionArray[resolutionIndex]=resolution;
                resolutionIndex++;
                resolutionDropdown.options.Add(new Dropdown.OptionData(option));
            }
        }

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    void ChangeResolution(int index)
    {
        Resolution[] resolutions = resolutionArray;

        if (index >= 0 && index < resolutions.Length)
        {
            Resolution selectedResolution = resolutions[index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            int IsFullScreen;

            if (Screen.fullScreen)
            {
                IsFullScreen = 1;
            }
            else
            {
                IsFullScreen = 0;
            }
            PlayerPrefs.SetInt("ResolutionFullScreen", IsFullScreen);
            PlayerPrefs.SetInt("ResolutionHeight", selectedResolution.height);
            PlayerPrefs.SetInt("ResolutionWidth", selectedResolution.width);
        }
    }

    void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        int IsFullScreen;
        if (isFullscreen)
        {
            IsFullScreen = 1;
        }
        else
        {
            IsFullScreen = 0;
        }
        PlayerPrefs.SetInt("ResolutionFullScreen", IsFullScreen);
    }
}
