using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public string[] BeforeChat1;
    string practicestring;
    public int CharPerSecnds;
    public Text PracticeText;
    int index;
    float interval;

    // Start is called before the first frame update
    void Start()
    {
        practicestring = "����ּ��� ��翩!! �� �༮�� �� �� ���̶��\n �� �༮�� �� �༮��...!!!";
        EffectStart();
    }

    void EffectStart()
    {
        index = 0;
        PracticeText.text = "";
        // ���� Ŀ�� �κ��� SetActive false�� �ٲ�

        interval = 1.0f / CharPerSecnds;
        Invoke("Effecting", interval);
    }
    void Effecting()
    {
        if(PracticeText.text== practicestring)
        {
            EffectEnd();
            return;
        }
        PracticeText.text += practicestring[index];
        index++;

        Invoke("Effecting", interval);
    }
    void EffectEnd()
    {
        // ���� Ŀ�� �κ��� SetActive true�� �ٲ�
    }

    // Effect�� ���� �ִϸ��̼� ����
}
