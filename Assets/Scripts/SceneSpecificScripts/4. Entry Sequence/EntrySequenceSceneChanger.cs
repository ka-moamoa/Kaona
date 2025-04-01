using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrySequenceSceneChanger : MonoBehaviour
{

    public Animator anim;

    public void MoveToScene(int sceneID){
        SceneManager.LoadScene(sceneID);
        GameDataManager.Instance.UpdateIntroSequenceDone(true);
    }

    public void FadeMoveToScene(){
        anim.Play ("Fade Out");
    }

    public void AssignMoku(string mokuID){
        PlayerPrefs.SetString("mokuID", mokuID);
    }

    public void AssignAudioID(int audioID){
        PlayerPrefs.SetInt("audioID", audioID);
    }
}
