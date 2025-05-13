using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorFadeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("CanMoveCharacter", 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CharacterSelectorIntroDone(){
        PlayerPrefs.SetInt("CanMoveCharacter", 1);
    }
}
