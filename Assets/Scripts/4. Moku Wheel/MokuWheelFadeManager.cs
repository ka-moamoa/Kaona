using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MokuProgressState
{
    public const int UNSTARTED = 0;
    public const int IN_PROGRESS = 1;
    public const int COMPLETE = 2;
}

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

    [Header("Game Data Lookup")]
    public MokuType mokuName;
    [Range(1, 9)]
    public int mokuQuestNum = 1; // 1-based in Inspector

    // Sprite state GameObjects
    private GameObject unstartedSprite;
    private GameObject inProgressSprite;
    private GameObject completeSprite;

    void Start()
    {
        mokuAudio = GetComponent<AudioSource>();
        fadeAnim = GameObject.FindWithTag("FadeCanvas").GetComponent<Animator>();

        // Find sprite state children by tag
        unstartedSprite = FindChildWithTag("UnstartedSprite");
        inProgressSprite = FindChildWithTag("InProgressSprite");
        completeSprite = FindChildWithTag("CompleteSprite");

        // Get the quest state and apply visuals
        int state = GetMokuQuestState();

        // Set correct sprite active
        if (unstartedSprite != null) unstartedSprite.SetActive(state == MokuProgressState.UNSTARTED);
        if (inProgressSprite != null) inProgressSprite.SetActive(state == MokuProgressState.IN_PROGRESS);
        if (completeSprite != null) completeSprite.SetActive(state == MokuProgressState.COMPLETE);
    }

    public void ChangeScene(int sceneNum)
    {
        this.sceneNum = sceneNum;

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

    private int GetMokuQuestState()
    {
        int index = mokuQuestNum - 1;
        if (index < 0 || index > 8)
        {
            Debug.LogError("Quest number must be between 1 and 9.");
            return -1;
        }

        return mokuName switch
        {
            MokuType.FF => GameDataManager.Instance.gameData.FF[index],
            MokuType.FE => GameDataManager.Instance.gameData.FE[index],
            MokuType.WS => GameDataManager.Instance.gameData.WS[index],
            MokuType.PB => GameDataManager.Instance.gameData.PB[index],
            MokuType.TM => GameDataManager.Instance.gameData.TM[index],
            MokuType.SS => GameDataManager.Instance.gameData.SS[index],
            _ => -1
        };
    }

    private GameObject FindChildWithTag(string tag)
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        Debug.LogWarning($"Child with tag '{tag}' not found on {gameObject.name}.");
        return null;
    }
}