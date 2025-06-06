using UnityEngine;
using UnityEngine.SceneManagement;
using AudioTextSynchronizer;
using AudioTextSynchronizer.Core;

public class BlankTimingSceneChanger : MonoBehaviour
{
    [Header("References")]
    public TextSynchronizer synchronizer;

    [Header("Scene Control")]
    public int targetSceneBuildIndex = 0; // Set to the scene you want to load

    private bool hasChanged = false;

    void Update()
    {
        if (hasChanged || synchronizer == null || !synchronizer.Source.isPlaying)
            return;

        float currentTime = synchronizer.Source.time;
        var timings = synchronizer.Timings?.Timings;

        if (timings == null || timings.Count == 0)
            return;

        // Skip ahead if not yet reached first timing
        if (currentTime < timings[0].StartPosition)
            return;

        Timing currentTiming = GetCurrentTiming(currentTime);
        if (currentTiming == null)
            return;

        if (string.IsNullOrWhiteSpace(currentTiming.Text))
        {
            Debug.LogWarning("Blank timing detected — switching scene.");

            GameDataManager.Instance.UpdateWSUnlocked(true);

            ResetAllLastOpenedFlags();

            GameDataManager.Instance.UpdateLastOpenedState(MokuType.WS, true);
            GameDataManager.Instance.UpdateWSTeleport(true);

            SceneManager.LoadScene(targetSceneBuildIndex);
            
            hasChanged = true;
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

    Timing GetCurrentTiming(float currentTime)
    {
        foreach (var timing in synchronizer.Timings.Timings)
        {
            if (timing.StartPosition <= currentTime && currentTime < timing.EndPosition)
                return timing;
        }
        return null;
    }
}


