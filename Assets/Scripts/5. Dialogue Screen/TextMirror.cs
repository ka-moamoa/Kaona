using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextMirror : MonoBehaviour
{
    public TextMeshProUGUI originalText;
    public RawImage mirroredImage;
    public Camera renderCamera;
    public RenderTexture renderTexture;

    void Start()
    {
        // Create a RenderTexture if not assigned
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        }

        // Assign the RenderTexture to the camera
        renderCamera.targetTexture = renderTexture;
    }

    void Update()
    {
        // Update the RenderTexture with the original TextMeshProUGUI text
        RenderTexture.active = renderTexture;
        renderCamera.Render();
        RenderTexture.active = null;

        // Display the RenderTexture on the Raw Image
        mirroredImage.texture = renderTexture;
    }
}
