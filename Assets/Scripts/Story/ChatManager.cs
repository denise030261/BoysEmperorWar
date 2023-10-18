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
        practicestring = "살려주세요 용사여!! 그 녀석이 곧 올 것이라네\n 그 녀석이 그 녀석이...!!!";
        EffectStart();
    }

    void EffectStart()
    {
        index = 0;
        PracticeText.text = "";
        // 엔드 커서 부분을 SetActive false로 바꿈

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
        // 엔드 커서 부분을 SetActive true로 바꿈
    }

    // Effect는 글자 애니메이션 구현
}
