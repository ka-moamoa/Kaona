using UnityEngine;
using TMPro;

public class MirrorTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI originalTextMeshPro;
    public TextMeshProUGUI mirroredTextMeshPro;

    void Start()
    {
        // Ensure the mirrored text initially matches the original text
        mirroredTextMeshPro.text = originalTextMeshPro.text;
        mirroredTextMeshPro.rectTransform.anchoredPosition = originalTextMeshPro.rectTransform.anchoredPosition;

        // Flip the mirrored text by 180 degrees around the y-axis
        mirroredTextMeshPro.rectTransform.localScale = new Vector3(1, 1, 1);

        // You might need to synchronize other properties like font, color, etc.
    }

    void Update()
    {
        // Synchronize content
        mirroredTextMeshPro.text = originalTextMeshPro.text;

        // Synchronize position
        mirroredTextMeshPro.rectTransform.anchoredPosition = originalTextMeshPro.rectTransform.anchoredPosition;

        // You might need to synchronize other properties like font, color, etc.
    }
}
