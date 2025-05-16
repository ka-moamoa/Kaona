using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MokuWheelFadeManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource mokuAudio;

    [Header("Scene Management")]
    public int sceneNum;
    public Animator fadeAnim;
    public bool changeInvoked = false;

    [Header("Moku Selection (Only One Should Be True)")]
    public bool FFSelected;
    public bool FESelected;
    public bool WSSelected;
    public bool PBSelected;
    public bool TMSelected;
    public bool SSSelected;

    void Start()
    {
        mokuAudio = GetComponent<AudioSource>();
    }

    public void ChangeScene(int sceneNum)
    {
        this.sceneNum = sceneNum;

        // Set the correct LastOpened value
        if (FFSelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.FF, true);
        if (FESelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.FE, true);
        if (WSSelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, true);
        if (PBSelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.PB, true);
        if (TMSelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.TM, true);
        if (SSSelected) GameDataManager.Instance.UpdateLastOpenedState(MokuType.SS, true);

        fadeAnim.SetTrigger("Fade Out");
        Invoke(nameof(InvokeChangeAfterDelay), 1f);
        changeInvoked = true;

        Debug.Log("Scene change triggered.");
    }

    private void InvokeChangeAfterDelay()
    {
        SceneManager.LoadScene(sceneNum);
    }
}
