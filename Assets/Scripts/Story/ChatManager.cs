using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : DataManager
{
    public int DelayChatSpeed = 5;
    string practicestring;
    public int CharPerSecnds;
    public Text PracticeText;
    int index;
    float interval;
    private Image ChatTitle;

    // DataManager���� �ν��Ͻ� �Ѱ��� ��� ���� ������ ����
        void Start()
    {
        ChatTitle = GetComponent<Image>();
        ChatInit();
    }

    private void Update()
    {
        if(DataManager.Instance.IsChat)
        {
            Debug.Log("��ȭ�� ���۵˴ϴ�");
            DataManager.Instance.IsChat = false;
            EffectStart();
        }
        else if (!DataManager.Instance.IsChat && DataManager.Instance.IsProgress &&
            Input.GetKeyUp(KeyCode.Return) && DataManager.Instance.PlaceData[DataManager.Instance.SceneNum + 1] != "")
        {
            ChatInit();
        }
    }

    IEnumerator DelayChat()
    {
        yield return new WaitForSeconds(DelayChatSpeed);
        ChatTitle.color = new Color(ChatTitle.color.r, ChatTitle.color.g, ChatTitle.color.b, 1);
        ChatTitle.sprite= Resources.Load<Sprite>
            ("Story/Ep1/ChatWindow/Before/" + ChatWindowData[DataManager.Instance.SceneNum]);
        EffectStart();
    }

    void EffectStart()
    {
        index = 0;
        PracticeText.text = "";

        interval = 1.0f / CharPerSecnds;
        Invoke("Effecting", interval);
    }
    void Effecting()
    {
        if(PracticeText.text== ChatData[DataManager.Instance.SceneNum])
        {
            EffectEnd();
            Debug.Log("��");
            return;
        }
        PracticeText.text += ChatData[DataManager.Instance.SceneNum][index];
        index++;

        Invoke("Effecting", interval);
    }
    void EffectEnd()
    {
        DataManager.Instance.IsProgress = true;
        DataManager.Instance.IsPlaceWindow = false;
    }
    // ���� �ִϸ��̼� ����

    public void ChatInit()
    {
        ChatTitle.color = new Color(ChatTitle.color.r, ChatTitle.color.g, ChatTitle.color.b, 0);
        PracticeText.text = "";
        StartCoroutine(DelayChat());
    }
}
