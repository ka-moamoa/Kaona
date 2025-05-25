using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MokuDialogueSceneChanger : MonoBehaviour
{
    public AudioSource mokuAudio;
    public bool changeInvoked = false;
    public Animator fadeAnim;

    void Start()
    {
        mokuAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!mokuAudio.isPlaying && !changeInvoked && Time.timeScale == 1)
        {
            fadeAnim.SetTrigger("Fade Out");
            Invoke("InvokeChangeAfterDelay", 1f);
            Debug.Log("STOP");
            changeInvoked = true;
        }
    }

    void InvokeChangeAfterDelay()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
