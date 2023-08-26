using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoLevel1 : MonoBehaviour
{
    // 두 개의 박스 콜라이더 중 하나가 충돌해야 할 대상의 태그를 설정하세요.
    private string Level;

    private bool collisionDetected = false;

    private void Start()
    {
        Debug.Log("실행되었습니다");
    }

    private void Update()
    {
        // 만약 충돌이 감지되었고 엔터 키가 눌렸다면 씬을 이동합니다.
        if (collisionDetected && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("충돌되었습니다");
            if(Level=="Level1")
            {
                Debug.Log(Level+"이 감지되었습니다");
                PlayerPrefs.SetString("SongName", "Feelin Like");
            }
            else if(Level == "Level2")
            {
                Debug.Log(Level + "이 감지되었습니다");
                PlayerPrefs.SetString("SongName", "Do or Not");
            }
            SceneManager.LoadScene("Game");

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 충돌 대상이 정확한 태그인지 확인합니다.
        if (other.CompareTag("Level1"))
        {
            Level = "Level1";
            collisionDetected = true;
        }
        else if(other.CompareTag("Level2"))
        {
            Level = "Level2";
            collisionDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 충돌이 해제되었을 때 상태를 초기화합니다.
        if (other.CompareTag("Level1") || other.CompareTag("Level2"))
        {
            Debug.Log("충돌해제");
            collisionDetected = false;
        }
    }
}
