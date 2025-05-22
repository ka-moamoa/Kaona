using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AutoGrowRotatedText : MonoBehaviour
{
    public float padding = 10f; // Extra space at the end
    public bool applyEveryFrame = true;

    private TextMeshProUGUI tmp;
    private RectTransform rectTransform;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        if (!applyEveryFrame)
            ResizeOnce();
    }

    void LateUpdate()
    {
        if (applyEveryFrame)
            ResizeOnce();
    }

    void ResizeOnce()
    {
        Vector2 preferred = tmp.GetPreferredValues(tmp.text);

        float zRotation = Mathf.Abs(rectTransform.eulerAngles.z % 360);
        Vector2 currentSize = rectTransform.sizeDelta;

        if (Mathf.Approximately(zRotation, 90f) || Mathf.Approximately(zRotation, 270f))
        {
            // Grow height (long dimension) while keeping width fixed
            rectTransform.sizeDelta = new Vector2(currentSize.x, preferred.x + padding);
        }
        else
        {
            // Standard orientation: grow width instead
            rectTransform.sizeDelta = new Vector2(preferred.x + padding, currentSize.y);
        }
    }
}
