using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Visualizer Sprites")]
    public Sprite optionASuccessIcon1;
    public Sprite optionASuccessIcon2;
    public Sprite optionASuccessIcon3;

    public Sprite optionBSuccessIcon1;
    public Sprite optionBSuccessIcon2;
    public Sprite optionBSuccessIcon3;

    public Sprite failIcon1;
    public Sprite failIcon2;
    public Sprite failIcon3;

    [Header("Option A Success Visualizer GameObjects")]
    public GameObject[] optionASuccessIcon1Objects;
    public GameObject[] optionASuccessIcon2Objects;
    public GameObject[] optionASuccessIcon3Objects;

    [Header("Option B Success Visualizer GameObjects")]
    public GameObject[] optionBSuccessIcon1Objects;
    public GameObject[] optionBSuccessIcon2Objects;
    public GameObject[] optionBSuccessIcon3Objects;

    [Header("Fail Visualizer GameObjects")]
    public GameObject[] failIcon1Objects;
    public GameObject[] failIcon2Objects;
    public GameObject[] failIcon3Objects;

    void Start()
    {
        HandleVisualizer(optionASuccessIcon1Objects, optionASuccessIcon1);
        HandleVisualizer(optionASuccessIcon2Objects, optionASuccessIcon2);
        HandleVisualizer(optionASuccessIcon3Objects, optionASuccessIcon3);

        HandleVisualizer(optionBSuccessIcon1Objects, optionBSuccessIcon1);
        HandleVisualizer(optionBSuccessIcon2Objects, optionBSuccessIcon2);
        HandleVisualizer(optionBSuccessIcon3Objects, optionBSuccessIcon3);

        HandleVisualizer(failIcon1Objects, failIcon1);
        HandleVisualizer(failIcon2Objects, failIcon2);
        HandleVisualizer(failIcon3Objects, failIcon3);

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

    private void HandleVisualizer(GameObject[] targets, Sprite sprite)
    {
        bool isValid = sprite != null && sprite.texture != null;

        foreach (GameObject obj in targets)
        {
            if (obj == null) continue;

            var image = obj.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.enabled = isValid;
            }
        }
    }

    public void FadeToMokuWheel() => fadeOutAnim.SetTrigger("Fade Out");

    public void FadeToMokuWheelAndMarkHealed()
    {
        int index = mokuQuestNum - 1;
        if (index < 0 || index > 8) return;

        switch (mokuName)
        {
            case MokuType.FF: GameDataManager.Instance.UpdateFFData(index, MokuProgressState.COMPLETE); break;
            case MokuType.FE: GameDataManager.Instance.UpdateFEData(index, MokuProgressState.COMPLETE); break;
            case MokuType.WS: GameDataManager.Instance.UpdateWSData(index, MokuProgressState.COMPLETE); break;
            case MokuType.PB: GameDataManager.Instance.UpdatePBData(index, MokuProgressState.COMPLETE); break;
            case MokuType.TM: GameDataManager.Instance.UpdateTMData(index, MokuProgressState.COMPLETE); break;
            case MokuType.SS: GameDataManager.Instance.UpdateSSData(index, MokuProgressState.COMPLETE); break;
        }

        fadeOutAnim.SetTrigger("Fade Out");
    }

    private int GetMokuQuestState()
    {
        int index = mokuQuestNum - 1;
        if (index < 0 || index > 8) return -1;

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
