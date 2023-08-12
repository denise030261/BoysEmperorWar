using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoLevel1 : MonoBehaviour
{
    // �� ���� �ڽ� �ݶ��̴� �� �ϳ��� �浹�ؾ� �� ����� �±׸� �����ϼ���.
    public string targetTag = "Level1";

    private bool collisionDetected = false;

    private void Start()
    {
        Debug.Log("����Ǿ����ϴ�");
    }

    private void Update()
    {
        // ���� �浹�� �����Ǿ��� ���� Ű�� ���ȴٸ� ���� �̵��մϴ�.
        if (collisionDetected && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("�浹�Ǿ����ϴ�"); 
            SceneManager.LoadScene("Story1");

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // �浹 ����� ��Ȯ�� �±����� Ȯ���մϴ�.
        if (other.CompareTag(targetTag))
        {
            collisionDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // �浹�� �����Ǿ��� �� ���¸� �ʱ�ȭ�մϴ�.
        if (other.CompareTag(targetTag))
        {
            Debug.Log("�浹����");
            collisionDetected = false;
        }
    }
}
