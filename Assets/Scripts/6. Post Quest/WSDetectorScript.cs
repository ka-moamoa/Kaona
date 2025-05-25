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
            SceneManager.LoadScene(targetSceneBuildIndex);
            hasChanged = true;
        }
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
