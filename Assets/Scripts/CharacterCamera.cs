using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    public GameObject Character;
    Transform CharacterTransform;

    void Start()
    {
        CharacterTransform=Character.transform;    
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(CharacterTransform.position.x, 0.33f, -1);
    }

}
