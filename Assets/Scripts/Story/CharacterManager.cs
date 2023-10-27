using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public Image LeftCharacter;
    public Image RightCharacter;

    // Start is called before the first frame update
    void Start()
    {
        CharacterInit();
    }

    // Update is called once per frame
    void Update()
    {
        if(!DataManager.Instance.IsChat || !DataManager.Instance.IsProgress)
        {

        }
    }

    private void CharacterInit()
    {
        LeftCharacter.color= new Color(LeftCharacter.color.r, LeftCharacter.color.g, LeftCharacter.color.b, 0);
        RightCharacter.color = new Color(RightCharacter.color.r, RightCharacter.color.g, RightCharacter.color.b, 0);
    }
}
