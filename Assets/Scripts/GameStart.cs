using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public GameObject PressLogo;
    public int PressLogoDelay = 3;
    public SpriteRenderer TitleLogo;
    public int TitleLogoDelay = 3;
    public int FadeSpeed = 5;
    public string BGMName;

    private bool DisplayTitle = false;
    private bool DisplayStart = false;
    private void Start()
    {
        MainAudioManager.Instance.PlayBGM(BGMName);
        StartCoroutine(LogoStart());
        StartCoroutine(PressLogoStart());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && DisplayStart)
        {
            PlayerPrefs.SetString("Scene", "Home");
            SceneManager.LoadScene("Loading(Imsi)");
        }

        if(DisplayTitle && TitleLogo.color.a<=1)
        {
            TitleLogo.color = new Color(1, 1, 1, TitleLogo.color.a + Time.deltaTime * FadeSpeed);
        }
    }

    IEnumerator PressLogoStart()
    {
        yield return new WaitForSeconds(PressLogoDelay);
        PressLogo.SetActive(true);  
        DisplayStart =true;
    } // 스타트 로고 나타내기

    IEnumerator LogoStart()
    {
        yield return new WaitForSeconds(TitleLogoDelay);
        DisplayTitle = true;
    } // 로고 나타내기
}
