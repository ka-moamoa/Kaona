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

    public GameObject startButton;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update()
    {
        DeviceOrientation currentOrientation = Input.deviceOrientation;
    }

    //Functions

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
