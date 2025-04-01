using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

using CandyCoded.HapticFeedback;

public class LoadMainMenu : MonoBehaviour
{

    public Animator anim;

    public VideoPlayer startVideo;
    public GameObject startVideoGameObject;
    public VideoPlayer rotatedStartVideo;
    public GameObject startButton;
    public bool firstVideoStarted = false;
    public bool secondVideoStarted = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Start is called before the first frame update
    void Start()
    {
        startVideo.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        DeviceOrientation currentOrientation = Input.deviceOrientation;

        // Debug.Log(currentOrientation);

        if (firstVideoStarted && startVideo.isPlaying == false){
            if (currentOrientation == DeviceOrientation.LandscapeLeft || currentOrientation == DeviceOrientation.FaceUp){

                if (secondVideoStarted == false){
                    startVideoGameObject.SetActive(false);
                    rotatedStartVideo.Play();
                    secondVideoStarted = true;
                }
            }
        }

        if(secondVideoStarted && rotatedStartVideo.isPlaying == false && rotatedStartVideo.time > 0){
            MoveToScene(1);
        }
    }


    private IEnumerator RunFunctionEveryHalfSecond()
    {
        float duration = 5.0f; // total duration for which we want to run the function
        float interval = 1f; // interval between each function call
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            Vibration();
            yield return new WaitForSeconds(interval);
        }
    }




    //Functions

    public void playVideo(){
        HapticFeedback.MediumFeedback();
        StartCoroutine(RunFunctionEveryHalfSecond());
        startVideo.Play();
        firstVideoStarted = true;
        startButton.SetActive(false);
    }

    public void playAnimation(){
        anim.Play("Pre-Start Animation");
    }

    public void MoveToScene(int sceneID){
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(sceneID);
    }

    public void Vibration(){
        HapticFeedback.HeavyFeedback();
        Debug.Log("Vibration Feedback");
    }

    public void HeavyVibration(){
        Handheld.Vibrate();
        Debug.Log("Vibration Feedback");
    }

}
