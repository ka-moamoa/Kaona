using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AncestorButtonScript : MonoBehaviour
{

    public Animator anim;



    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fadeOut(){
        anim.Play("Fade Out");
    }

    public void nextScene(int sceneID){
        SceneManager.LoadScene(sceneID);
    }

}
