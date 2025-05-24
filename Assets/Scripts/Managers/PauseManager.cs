using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public Animator pauseUI;
    public Animator fadeAnim;

    public int sceneNum;

    public bool isPaused = false;

    public int questSelectSceneNum;

    // Start is called before the first frame update
    void Start()
    {
        pauseUI = GameObject.FindGameObjectWithTag("Pause UI").GetComponent<Animator>();
        fadeAnim = GameObject.FindWithTag("FadeCanvas").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        if (pauseUI != null)
        {
            pauseUI.Play("Pause Menu Open");
        }

        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        if (pauseUI != null)
        {
            pauseUI.Play("Pause Menu Close");
        }

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void MainMenu()
    {
        fadeAnim.SetTrigger("Fade Out");
        Invoke(nameof(InvokeChangeAfterDelay), 1f);

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    private void InvokeChangeAfterDelay()
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    public void QuestSelector()
    {
        fadeAnim.SetTrigger("Fade Out");
        fadeAnim.SetTrigger("FadeOut");
        Invoke(nameof(InvokeChangeAfterDelayQuest), 1f);

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    private void InvokeChangeAfterDelayQuest()
    {
        SceneManager.LoadScene(questSelectSceneNum);
    }

    public void ReloadCurrentScene()
    {
        fadeAnim.SetTrigger("Fade Out");
        fadeAnim.SetTrigger("FadeOut");
        Invoke(nameof(InvokeReloadCurrentScene), 1f);

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    private void InvokeReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

}
