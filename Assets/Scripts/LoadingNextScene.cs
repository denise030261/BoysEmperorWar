using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingNextScene : MonoBehaviour
{
    public int WaitSeconds = 10;

    void Start()
    {
        StartCoroutine(NextScene());
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(WaitSeconds);
        SceneManager.LoadScene("StageSelect");
    } // 로고 나타내기
}
