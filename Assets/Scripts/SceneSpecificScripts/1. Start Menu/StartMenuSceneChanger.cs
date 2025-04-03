using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuSceneChanger : MonoBehaviour
{

    public Animator anim;

    public Sprite MainNewGameSprite;
    public Sprite MainContinueGameSprite;

    public GameObject startButton;

    public GameObject newGameButton;

    public Sprite SmallNewGameSprite;
    public Sprite SmallUnavailableGameSprite;


    public GameObject resetJourneyOverlay;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (newGameButton != null){
            if (GameDataManager.Instance.gameData.introSequenceDone){
                // newGameButton.GetComponent<Image>().sprite = SmallNewGameSprite;
                newGameButton.GetComponent<Button>().interactable = true;
            }else{
                // newGameButton.GetComponent<Image>().sprite = SmallUnavailableGameSprite;
                newGameButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    void Start(){
        if (startButton != null){
            if (GameDataManager.Instance.gameData.introSequenceDone){
                startButton.GetComponent<Image>().sprite = MainContinueGameSprite;
            }else{
                startButton.GetComponent<Image>().sprite = MainNewGameSprite;
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

    public void LoadAbout(){
        anim.Play ("LoadAbout");
    }

    public void UnLoadAbout(){
        anim.SetTrigger ("UnloadAbout");
        anim.Play("Default");
    }

    public void OpenResetJourneyOverlay(){
        resetJourneyOverlay.SetActive(true);
    }

    public void CloseResetJourneyOverlay(){
        resetJourneyOverlay.SetActive(true);
    }

    public void ResetDataFadeMoveToScene(){
        GameDataManager.Instance.ResetGameData();
        anim.Play ("Fade Out");
    }

    public void AssignMoku(string mokuID){
        PlayerPrefs.SetString("mokuID", mokuID);
    }

    public void AssignAudioID(int audioID){
        PlayerPrefs.SetInt("audioID", audioID);
    }
}
