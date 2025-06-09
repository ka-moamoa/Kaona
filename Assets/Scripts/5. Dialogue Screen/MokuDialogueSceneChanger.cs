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

    [Header("Scene Override Settings")]
    public bool overrideFF = false;
    public bool overrideFE = false;
    public bool overrideWS = false;
    public bool overridePB = false;
    public bool overrideTM = false;
    public bool overrideSS = false;

    public int overrideSceneIndex = 0;

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
        if (overrideFF)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.FF, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else if (overrideFE)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.FE, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else if (overrideWS)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.WS, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else if (overridePB)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.PB, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else if (overrideTM)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.TM, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else if (overrideSS)
        {
            GameDataManager.Instance.UpdateRulerPanelOpenOnReturn(MokuType.SS, true);
            SceneManager.LoadScene(overrideSceneIndex);
        }
        else
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
