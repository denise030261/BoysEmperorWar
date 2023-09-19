using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public GameObject TitleLogo;
    public int TitleLogoDelay = 3;
    private bool DisplayStart = false;
    private void Start()
    {
      StartCoroutine(LogoStart());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && DisplayStart)
        {
            SceneManager.LoadScene("StageSelect");
        }
    }

    IEnumerator LogoStart()
    {
        yield return new WaitForSeconds(TitleLogoDelay);
        TitleLogo.SetActive(true);  
        DisplayStart =true;
    } // 로고 나타내기
}
