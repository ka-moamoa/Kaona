using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoScrollATS : MonoBehaviour
{
    public ScrollRect scrollRect;
    public AudioSource audioSource;
    public float scrollSpeedMultiplier = 1f;
    public float returnScrollDuration = 0.2f; // quick smooth scroll
    [Range(0f, 0.2f)] public float scrollStartBuffer = 0.05f;

    private float totalDuration;
    private bool isScrolling = false;
    private float scrollStartTime = float.MaxValue;
    private bool hasReset = false;
    private Image scrollbarBackgroundImage;
    private Image scrollbarHandleImage;

    void Start()
    {
        if (audioSource.clip != null)
        {
            totalDuration = audioSource.clip.length;
            CacheScrollbarImages();
            StartCoroutine(DelayedEstimateScrollStartTime());
        }
    }

    void CacheScrollbarImages()
    {
        if (scrollRect.verticalScrollbar != null)
        {
            scrollbarBackgroundImage = scrollRect.verticalScrollbar.GetComponent<Image>();
            if (scrollRect.verticalScrollbar.handleRect != null)
            {
                scrollbarHandleImage = scrollRect.verticalScrollbar.handleRect.GetComponent<Image>();
            }
        }
    }

    void Update()
    {
        // Hide the scrollbar visuals every frame unless it's reset time
        if (!hasReset)
        {
            if (scrollbarBackgroundImage) scrollbarBackgroundImage.enabled = false;
            if (scrollbarHandleImage) scrollbarHandleImage.enabled = false;
        }

        if (audioSource.isPlaying && totalDuration > 0)
        {
            float time = audioSource.time;

            if (time >= scrollStartTime)
            {
                float progress = (time - scrollStartTime) / (totalDuration - scrollStartTime);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1 - progress * scrollSpeedMultiplier);
                isScrolling = true;
            }
            else
            {
                scrollRect.verticalNormalizedPosition = 1f;
                isScrolling = false;
            }

            hasReset = false;
        }
        else if (!audioSource.isPlaying && isScrolling && !hasReset)
        {
            StartCoroutine(SmoothScrollToTop());
            isScrolling = false;
            hasReset = true;
        }
    }

    IEnumerator DelayedEstimateScrollStartTime()
    {
        yield return new WaitForSeconds(0.5f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        if (contentHeight <= viewportHeight)
        {
            scrollStartTime = float.MaxValue;
            scrollRect.enabled = false;
        }
        else
        {
            float visibleRatio = viewportHeight / contentHeight;
            scrollStartTime = totalDuration * Mathf.Clamp01(visibleRatio - scrollStartBuffer);
            scrollRect.enabled = true;
        }
    }

    IEnumerator SmoothScrollToTop()
    {
        yield return null; // Wait for layout

        // Re-enable scrollbar visuals for final scroll-up
        if (scrollbarBackgroundImage) scrollbarBackgroundImage.enabled = true;
        if (scrollbarHandleImage) scrollbarHandleImage.enabled = true;

        float startPos = scrollRect.verticalNormalizedPosition;
        float elapsed = 0f;

        while (elapsed < returnScrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnScrollDuration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, 1f, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.StopMovement();

        Canvas.ForceUpdateCanvases();
        scrollRect.OnScroll(null);

        this.enabled = false;
    }
}
