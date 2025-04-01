using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuSceneChanger : MonoBehaviour
{

    public Animator anim;

    public Sprite NewGameSprite;
    public Sprite ContinueGameSprite;

    public GameObject startButton;

    void Start(){
        if (startButton != null){
            if (GameDataManager.Instance.gameData.introSequenceDone){
                startButton.GetComponent<Image>().sprite = ContinueGameSprite;
            }else{
                startButton.GetComponent<Image>().sprite = NewGameSprite;
            }
        }
    }

    public void MoveToScene(int sceneID){
        SceneManager.LoadScene(sceneID);
    }

    public void MoveToSceneMain(int sceneID){
        Debug.Log(GameDataManager.Instance.gameData.introSequenceDone);
        if(GameDataManager.Instance.gameData.introSequenceDone == false){
            SceneManager.LoadScene(sceneID);
        }else{
            SceneManager.LoadScene(5);
        }
    }

    public void FadeMoveToScene(){
        anim.Play ("Fade Out");
    }

    public void LoadCredits(){
        anim.Play ("Load Credits In");
    }

    public void UnLoadCredits(){
        anim.Play("Unload Credits");
    }

    public void StopCredits(){
        anim.SetTrigger("UnloadCredits");
        anim.Play("Default");
    }

    public void AssignMoku(string mokuID){
        PlayerPrefs.SetString("mokuID", mokuID);
    }

    public void AssignAudioID(int audioID){
        PlayerPrefs.SetInt("audioID", audioID);
    }
}
