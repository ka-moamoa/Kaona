using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MokuWheelFadeManager : MonoBehaviour
{
    public AudioSource mokuAudio;
    public String sceneName;

    public Boolean changeInvoked = false;

    public Animator fadeAnim;

    // Start is called before the first frame update
    void Start()
    {
        mokuAudio = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InvokeChangeAfterDelay(){
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(String sceneName){
        this.sceneName = sceneName;
        fadeAnim.SetTrigger("Fade Out");
        Invoke("InvokeChangeAfterDelay", 1);
        Debug.Log("STOP");
        changeInvoked = true;
    }
}
