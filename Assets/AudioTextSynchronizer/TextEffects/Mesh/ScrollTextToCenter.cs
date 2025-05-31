using UnityEngine;
using AudioTextSynchronizer;
using AudioTextSynchronizer.Core;
using TMPro;

public class ScrollTextToCenter : MonoBehaviour
{
    [Header("References")]
    public TextSynchronizer synchronizer;
    public Transform textObject;
    public Transform centerTarget;
    public TMP_Text tmpText;

    [Header("Scroll Settings")]
    public float smoothing = 2f;
    public float scrollSpeed = 20f;

    private Vector3 velocity = Vector3.zero;
    private Timing lastTiming = null;
    private float targetX;

    void Start()
    {
        if (!synchronizer || !textObject || !centerTarget || !tmpText)
        {
            Debug.LogError("ScrollTextToCenter: Missing references.");
            enabled = false;
            return;
        }

        tmpText.ForceMeshUpdate();
        targetX = textObject.position.x;
    }

    void Update()
    {
        if (!synchronizer.Source.isPlaying)
            return;

        float currentTime = synchronizer.Source.time;
        var timings = synchronizer.Timings.Timings;

        if (timings == null || timings.Count == 0)
            return;

        if (currentTime < timings[0].StartPosition)
            return;

        Timing currentTiming = GetCurrentTiming(currentTime);
        if (currentTiming == null)
            return;

        if (string.IsNullOrWhiteSpace(currentTiming.Text))
        {
            float newX = textObject.position.x - scrollSpeed * Time.deltaTime;
            textObject.position = new Vector3(newX, textObject.position.y, textObject.position.z);
            return;
        }

        if (!ReferenceEquals(currentTiming, lastTiming))
        {
            lastTiming = currentTiming;
            tmpText.ForceMeshUpdate();
            targetX = CalculateTargetX(currentTiming);
        }

        float newPosX = Mathf.SmoothDamp(textObject.position.x, targetX, ref velocity.x, 1f / smoothing);
        textObject.position = new Vector3(newPosX, textObject.position.y, textObject.position.z);
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

    float CalculateTargetX(Timing timing)
    {
        string fullText = synchronizer.Timings.Text;
        int timingStartIndex = fullText.IndexOf(timing.Text);
        if (timingStartIndex < 0)
        {
            Debug.LogWarning($"Text '{timing.Text}' not found in full transcript.");
            return textObject.position.x;
        }

        int centerCharIndex = timingStartIndex + timing.Text.Length / 2;
        if (centerCharIndex >= tmpText.textInfo.characterCount)
            return textObject.position.x;

        // Try to find the nearest visible character around the centerCharIndex
        int range = 3;
        for (int offset = 0; offset <= range; offset++)
        {
            foreach (int i in new int[] { centerCharIndex - offset, centerCharIndex + offset })
            {
                if (i >= 0 && i < tmpText.textInfo.characterCount)
                {
                    var charInfo = tmpText.textInfo.characterInfo[i];
                    if (charInfo.isVisible)
                    {
                        Vector3 charLocalCenter = (charInfo.bottomLeft + charInfo.topRight) / 2f;
                        Vector3 charWorldPos = tmpText.transform.TransformPoint(charLocalCenter);
                        float offsetX = centerTarget.position.x - charWorldPos.x;
                        return textObject.position.x + offsetX;
                    }
                }
            }
        }

        // If no visible character found, return current position
        return textObject.position.x;
    }
}
