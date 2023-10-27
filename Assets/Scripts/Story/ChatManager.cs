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

    public Image LeftCharacter;
    public Image RightCharacter;

    // DataManager���� �ν��Ͻ� �Ѱ��� ��� ���� ������ ����
    void Start()
    {
        ChatTitle = GetComponent<Image>();
        ChatInit();
        CharacterInit();
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

        if(ChatTitle.color.a==1f)
        {
            if(LeftCharacterData[DataManager.Instance.SceneNum] == "����")
            {
                LeftCharacter.color = new Color(LeftCharacter.color.r, LeftCharacter.color.g, LeftCharacter.color.b, 0);
            }
            else if(LeftCharacterData[DataManager.Instance.SceneNum] != "")
            {
                LeftCharacter.color = new Color(LeftCharacter.color.r, LeftCharacter.color.g, LeftCharacter.color.b, 1);
                LeftCharacter.sprite = Resources.Load<Sprite>
            ("Story/Ep1/LeftCharacter/Before/" + LeftCharacterData[DataManager.Instance.SceneNum]);
            }

            if(RightCharacterData[DataManager.Instance.SceneNum] == "����")
            {
                RightCharacter.color = new Color(RightCharacter.color.r, RightCharacter.color.g, RightCharacter.color.b, 0);
            }
            else if(RightCharacterData[DataManager.Instance.SceneNum] != "")
            {
                RightCharacter.color = new Color(RightCharacter.color.r, RightCharacter.color.g, RightCharacter.color.b, 1);
                RightCharacter.sprite = Resources.Load<Sprite>
            ("Story/Ep1/RightCharacter/Before/" + RightCharacterData[DataManager.Instance.SceneNum]);
            }
        }
        else if (ChatTitle.color.a == 0f)
        {
            LeftCharacter.color = new Color(LeftCharacter.color.r, LeftCharacter.color.g, LeftCharacter.color.b, 0);
            RightCharacter.color = new Color(RightCharacter.color.r, RightCharacter.color.g, RightCharacter.color.b, 0);
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

    private void CharacterInit()
    {
        LeftCharacter.color = new Color(LeftCharacter.color.r, LeftCharacter.color.g, LeftCharacter.color.b, 0);
        RightCharacter.color = new Color(RightCharacter.color.r, RightCharacter.color.g, RightCharacter.color.b, 0);
    }
}
