using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingNextScene : MonoBehaviour
{
    public int WaitSeconds = 10;
    private string scene;

    void Start()
    {
        MainAudioManager.Instance.StopBGM();
        scene = PlayerPrefs.GetString("Scene", "Home");
        StartCoroutine(NextScene());
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(WaitSeconds);
        if(scene == "Home")
            SceneManager.LoadScene("StageSelect");
        else if(scene == "StageSelect")
            SceneManager.LoadScene("Story");
    } // 로고 나타내기
}
