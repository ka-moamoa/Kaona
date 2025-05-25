using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeAndSceneManager : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public GameObject objectToDisable;
    public Animator fadeOutAnim;

    [Header("Scene")]
    public float fadeDelay = 1f;

    [Header("Quest Data")]
    public MokuType mokuName;
    [Range(1, 9)] public int mokuQuestNum = 1;

    private bool hasDisabled = false;

    void Update()
    {
        if (!hasDisabled && audioSource != null && objectToDisable != null)
        {
            if (!audioSource.isPlaying && audioSource.time > 0f)
            {
                objectToDisable.SetActive(false);
                hasDisabled = true;
                Debug.Log("Audio finished. Object disabled.");
            }
        }
    }

    // Call this from a UI Button
    public void FadeAndGoToScene(int sceneBuildIndex)
    {
        MarkMokuQuestComplete();
        GameDataManager.Instance.UpdateWSUnlocked(true);
        ResetAllLastOpenedFlags();
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, true);

        if (fadeOutAnim != null)
        {
            fadeOutAnim.SetTrigger("Fade Out");
            StartCoroutine(LoadSceneAfterDelay(sceneBuildIndex, fadeDelay));
        }
        else
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }

    private IEnumerator LoadSceneAfterDelay(int buildIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(buildIndex);
    }

    private void MarkMokuQuestComplete()
    {
        int index = mokuQuestNum - 1;
        if (index < 0 || index >= 9)
        {
            Debug.LogError("Quest index out of bounds");
            return;
        }

        switch (mokuName)
        {
            case MokuType.FF: GameDataManager.Instance.UpdateFFData(index, MokuProgressState.COMPLETE); break;
            case MokuType.FE: GameDataManager.Instance.UpdateFEData(index, MokuProgressState.COMPLETE); break;
            case MokuType.WS: GameDataManager.Instance.UpdateWSData(index, MokuProgressState.COMPLETE); break;
            case MokuType.PB: GameDataManager.Instance.UpdatePBData(index, MokuProgressState.COMPLETE); break;
            case MokuType.TM: GameDataManager.Instance.UpdateTMData(index, MokuProgressState.COMPLETE); break;
            case MokuType.SS: GameDataManager.Instance.UpdateSSData(index, MokuProgressState.COMPLETE); break;
        }
    }

    private void ResetAllLastOpenedFlags()
    {
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.FF, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.FE, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.PB, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.TM, false);
        GameDataManager.Instance.UpdateLastOpenedState(MokuType.SS, false);
    }
}
