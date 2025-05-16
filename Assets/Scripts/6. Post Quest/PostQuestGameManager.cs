using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostQuestGameManager : MonoBehaviour
{
    public Animator animOptionA;
    public Animator animOptionB;
    
    public Animator animOptionAFail;
    public Animator animOptionASuccess;
    public Animator animOptionBFail;
    public Animator animOptionBSuccess;

    public Animator animOverall;

    public Animator fadeOutAnim;

    public AudioSource optionAAudio;
    
    public AudioSource optionAFailAudio;
    public AudioSource optionASuccessAudio;

    public AudioSource optionBAudio;
    public AudioSource optionBFailAudio;
    public AudioSource optionBSuccessAudio;

    public bool optionAAudioPlayed = false;
    public bool optionAFailAudioPlayed = false;

    public bool optionASuccessAudioPlayed = false;
    public bool optionBAudioPlayed = false;
    public bool optionBFailAudioPlayed = false;

    public bool optionBSuccessAudioPlayed = false;

    public GameObject optionACompleteOverlay;
    

    // Start is called before the first frame update
    void Start()
    {
        optionAAudio.Play();

        ////CHANGE VALUE
        //if (GameDataManager.Instance.gameData.FF[0]){
        //    optionACompleteOverlay.SetActive(true);
        //    optionAAudio.enabled = false;
        //}
        //else{
        //    optionACompleteOverlay.SetActive(false);
        //    optionAAudio.enabled = true;
        //    Debug.Log("QUEST COMPLETE");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (optionAAudio.isPlaying == false && optionBAudio.isPlaying != true && optionAAudioPlayed == false){
            animOptionA.SetTrigger("End Animation");
            Debug.Log("AUDIO A STOPPED");

            animOptionB.SetTrigger("Start Animation");
            optionBAudio.Play();
            optionAAudioPlayed = true;
        }

        if (optionBAudio.isPlaying == false && optionAAudioPlayed == true && optionBAudioPlayed == false){
            animOptionB.SetTrigger("End Animation");
            Debug.Log("AUDIO B STOPPED");

            optionBAudioPlayed = true;

            animOverall.SetTrigger("Show Requirements");
        }

        if (optionAFailAudioPlayed && optionAFailAudio.isPlaying == false){
            optionAFailAudioPlayed = false;

            animOptionAFail.SetTrigger("End Animation");
            animOverall.SetTrigger("Fail Option A - End Audio");
        }

        if (optionASuccessAudioPlayed && optionASuccessAudio.isPlaying == false){
            optionASuccessAudioPlayed = false;

            animOptionASuccess.SetTrigger("End Animation");
            animOverall.SetTrigger("Success Option A - End Audio");
        }

        if (optionBFailAudioPlayed && optionBFailAudio.isPlaying == false){
            optionBFailAudioPlayed = false;

            animOptionBFail.SetTrigger("End Animation");
            animOverall.SetTrigger("Fail Option B - End Audio");
        }

        if (optionBSuccessAudioPlayed && optionBSuccessAudio.isPlaying == false){
            optionBSuccessAudioPlayed = false;

            animOptionASuccess.SetTrigger("End Animation");
            animOverall.SetTrigger("Success Option B - End Audio");
        }
    }

        public void OpenOptionA(){
            animOverall.SetTrigger("Open Option A");
        }

        public void FailOptionA(){
            animOverall.SetTrigger("Fail Option A");
            optionAFailAudio.Play();
            optionAFailAudioPlayed = true;
        }

        public void SuccessOptionA(){
            animOverall.SetTrigger("Success Option A");
            optionASuccessAudio.Play();
            optionASuccessAudioPlayed = true;

            //CHANGE VALUE
            //GameDataManager.Instance.UpdateFFData(0, true);
        }

        public void FadeToMokuWheel(){
            fadeOutAnim.SetTrigger("Fade Out");
        }

        public void CloseOptionA(){
            animOverall.SetTrigger("Close Option A");
        }

        public void OpenOptionB(){
            animOverall.SetTrigger("Open Option B");
        }

        public void FailOptionB(){
            animOverall.SetTrigger("Fail Option B");
            optionBFailAudio.Play();
            optionBFailAudioPlayed = true;
        }

        public void SuccessOptionB(){
            animOverall.SetTrigger("Success Option B");
            optionBSuccessAudio.Play();
            optionBSuccessAudioPlayed = true;
        }

        public void CloseOptionB(){
            animOverall.SetTrigger("Close Option B");
        }
}
