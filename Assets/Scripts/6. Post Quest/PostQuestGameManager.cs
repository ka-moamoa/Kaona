using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioTextSynchronizer;

public class PostQuestGameManager : MonoBehaviour
{
    [Header("Moku Quest Lookup")]
    public MokuType mokuName;
    [Range(1, 9)] public int mokuQuestNum = 1;

    [Header("Animators")]
    public Animator animOptionA;
    public Animator animOptionB;
    public Animator animOptionAFail;
    public Animator animOptionASuccess;
    public Animator animOptionBFail;
    public Animator animOptionBSuccess;
    public Animator animOverall;
    public Animator fadeOutAnim;

    [Header("Audio Sources")]
    public AudioSource optionAAudio;
    public AudioSource optionAFailAudio;
    public AudioSource optionASuccessAudio;
    public AudioSource optionBAudio;
    public AudioSource optionBFailAudio;
    public AudioSource optionBSuccessAudio;

    [Header("Audio State Flags")]
    public bool optionAAudioPlayed = false;
    public bool optionAFailAudioPlayed = false;
    public bool optionASuccessAudioPlayed = false;
    public bool optionBAudioPlayed = false;
    public bool optionBFailAudioPlayed = false;
    public bool optionBSuccessAudioPlayed = false;

    [Header("Visual Overlays")]
    public GameObject optionACompleteOverlay;

    [Header("Text Synchronizers")]
    [SerializeField] private TextSynchronizer audioASuccessTextSynchronizer;
    [SerializeField] private TextSynchronizer audioAFailTextSynchronizer;
    [SerializeField] private TextSynchronizer audioBSuccessTextSynchronizer;
    [SerializeField] private TextSynchronizer audioBFailTextSynchronizer;

    void Start()
    {
        int questState = GetMokuQuestState();

        if (questState == MokuProgressState.COMPLETE)
        {
            optionACompleteOverlay.SetActive(true);
            optionAAudio.enabled = false;
        }
        else
        {
            optionACompleteOverlay.SetActive(false);
            optionAAudio.enabled = true;
            optionAAudio.Play();
            Debug.Log("QUEST INCOMPLETE — starting Option A audio.");
        }
    }

    void Update()
    {
        if (!optionAAudio.isPlaying && !optionBAudio.isPlaying && !optionAAudioPlayed && Time.timeScale == 1f)
        {
            animOptionA.SetTrigger("End Animation");
            animOptionB.SetTrigger("Start Animation");
            optionBAudio.Play();
            optionAAudioPlayed = true;
        }

        if (!optionBAudio.isPlaying && optionAAudioPlayed && !optionBAudioPlayed && Time.timeScale == 1f)
        {
            animOptionB.SetTrigger("End Animation");
            optionBAudioPlayed = true;
            animOverall.SetTrigger("Show Requirements");
        }

        if (optionAFailAudioPlayed && !optionAFailAudio.isPlaying && Time.timeScale == 1f)
        {
            optionAFailAudioPlayed = false;
            animOptionAFail.SetTrigger("End Animation");
            animOverall.SetTrigger("Fail Option A - End Audio");
        }

        if (optionASuccessAudioPlayed && !optionASuccessAudio.isPlaying && Time.timeScale == 1f)
        {
            optionASuccessAudioPlayed = false;
            animOptionASuccess.SetTrigger("End Animation");
            animOverall.SetTrigger("Success Option A - End Audio");
        }

        if (optionBFailAudioPlayed && !optionBFailAudio.isPlaying && Time.timeScale == 1f)
        {
            optionBFailAudioPlayed = false;
            animOptionBFail.SetTrigger("End Animation");
            animOverall.SetTrigger("Fail Option B - End Audio");
        }

        if (optionBSuccessAudioPlayed && !optionBSuccessAudio.isPlaying && Time.timeScale == 1f)
        {
            optionBSuccessAudioPlayed = false;
            animOptionASuccess.SetTrigger("End Animation");
            animOverall.SetTrigger("Success Option B - End Audio");
        }
    }

    public void OpenOptionA() => animOverall.SetTrigger("Open Option A");
    public void CloseOptionA() => animOverall.SetTrigger("Close Option A");
    public void OpenOptionB() => animOverall.SetTrigger("Open Option B");
    public void CloseOptionB() => animOverall.SetTrigger("Close Option B");

    public void FailOptionA()
    {
        animOverall.SetTrigger("Fail Option A");
        StartCoroutine(WaitThenPlay(audioAFailTextSynchronizer, () => optionAFailAudioPlayed = true));
    }

    public void SuccessOptionA()
    {
        animOverall.SetTrigger("Success Option A");
        StartCoroutine(WaitThenPlay(audioASuccessTextSynchronizer, () => optionASuccessAudioPlayed = true));
    }

    public void FailOptionB()
    {
        animOverall.SetTrigger("Fail Option B");
        StartCoroutine(WaitThenPlay(audioBFailTextSynchronizer, () => optionBFailAudioPlayed = true));
    }

    public void SuccessOptionB()
    {
        animOverall.SetTrigger("Success Option B");
        StartCoroutine(WaitThenPlay(audioBSuccessTextSynchronizer, () => optionBSuccessAudioPlayed = true));
    }

    private IEnumerator WaitThenPlay(TextSynchronizer synchronizer, System.Action setFlag)
    {
        yield return new WaitUntil(() => synchronizer.gameObject.activeInHierarchy);
        yield return new WaitForSeconds(0.1f);
        synchronizer.Play(true);
        setFlag?.Invoke();
    }

    public void FadeToMokuWheel() => fadeOutAnim.SetTrigger("Fade Out");

    public void FadeToMokuWheelAndMarkHealed()
    {
        int index = mokuQuestNum - 1;
        if (index < 0 || index > 8)
        {
            Debug.LogError("Quest number must be between 1 and 9.");
        }
        else
        {
            switch (mokuName)
            {
                case MokuType.FF: GameDataManager.Instance.UpdateFFData(index, MokuProgressState.COMPLETE); break;
                case MokuType.FE: GameDataManager.Instance.UpdateFEData(index, MokuProgressState.COMPLETE); break;
                case MokuType.WS: GameDataManager.Instance.UpdateWSData(index, MokuProgressState.COMPLETE); break;
                case MokuType.PB: GameDataManager.Instance.UpdatePBData(index, MokuProgressState.COMPLETE); break;
                case MokuType.TM: GameDataManager.Instance.UpdateTMData(index, MokuProgressState.COMPLETE); break;
                case MokuType.SS: GameDataManager.Instance.UpdateSSData(index, MokuProgressState.COMPLETE); break;
            }

            Debug.Log($"Set {mokuName} quest {index + 1} to COMPLETE.");
        }

        fadeOutAnim.SetTrigger("Fade Out");
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
}
