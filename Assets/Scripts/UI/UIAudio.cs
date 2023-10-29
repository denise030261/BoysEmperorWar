using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudio : MonoBehaviour
{
    public Slider slider_BGM;
    public Slider slider_SFX;
    public Text text_BGM;
    public Text text_SFX;

    private void Awake()
    {
        CurrentState();
    }
    private void Update()
    {
        PlayerPrefs.SetFloat("BGM", slider_BGM.value);
        PlayerPrefs.SetFloat("SFX", slider_SFX.value);
        ChangeText();
    }
    private void OnEnable()
    {
        CurrentState();
    }

    private void CurrentState()
    {
        slider_BGM.value = PlayerPrefs.GetFloat("BGM", 0.5f);
        slider_SFX.value = PlayerPrefs.GetFloat("SFX", 0.5f);
        ChangeText();
    }

    private void ChangeText()
    {
        text_BGM.text = ((int)(slider_BGM.value * 100)).ToString();
        text_SFX.text = ((int)(slider_SFX.value * 100)).ToString();
    }
}
