using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButtons : MonoBehaviour
{

    Animator overallAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenOptionA(){
        overallAnim.SetTrigger("Open Option A");
    }
}
