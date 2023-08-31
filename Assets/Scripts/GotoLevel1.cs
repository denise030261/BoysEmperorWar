using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GotoLevel1 : MonoBehaviour
{
    private string Level; // ���������� �̸�

    private bool collisionDetected = false; //�ݸ��� ���� ����

    private void Awake()
    {
        LevelInit();
    }

    private void Update()
    {
        // ���� �浹�� �����Ǿ��� ���� Ű�� ���ȴٸ� ���� �̵��մϴ�.
        if (collisionDetected && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("�浹�Ǿ����ϴ�");
            if(Level=="Level1")
            {
                Debug.Log(Level+"�� �����Ǿ����ϴ�");
                PlayerPrefs.SetString("SongName", "Feelin Like");
            }
            else if(Level == "Level2")
            {
                Debug.Log(Level + "�� �����Ǿ����ϴ�");
                PlayerPrefs.SetString("SongName", "Do or Not");
            }
            SceneManager.LoadScene("Game");

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // �浹 ����� ��Ȯ�� �±����� Ȯ���մϴ�.
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
        // �浹�� �����Ǿ��� �� ���¸� �ʱ�ȭ�մϴ�.
        if (other.CompareTag("Level1") || other.CompareTag("Level2"))
        {
            Debug.Log("�浹����");
            collisionDetected = false;
        }
    }

    private void LevelInit()
    {
        Level = PlayerPrefs.GetString("SongName", "Feelin Like");
        PlayerPrefs.SetString("SongName", Level);
    }
}
