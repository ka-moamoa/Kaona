using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MokuAudioScreenManager : MonoBehaviour
{

    public TMPro.TextMeshProUGUI mokuText;
    public TMPro.TextMeshProUGUI audioQuest;
    public Image centerImage;
    public Image pauseImage;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("mokuID" + PlayerPrefs.GetString("mokuID"));
        Debug.Log("audioID" + PlayerPrefs.GetInt("audioID"));

        mokuText.text = "Moku: " + PlayerPrefs.GetString("mokuID");
        audioQuest.text = "Audio Quest: " + PlayerPrefs.GetInt("audioID");

        if (centerImage != null){
            LoadAndAssignCenterImage();
            LoadAndAssignPauseImage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadAndAssignCenterImage()
    {
        string imagePath = "Audio Player Assets/" + PlayerPrefs.GetString("mokuID") + "/" + PlayerPrefs.GetString("mokuID") + "_" + PlayerPrefs.GetInt("audioID");
        // Load the image from the Resources folder
        Sprite loadedImage = Resources.Load<Sprite>(imagePath);

        // Check if the image is loaded successfully
        if (loadedImage != null)
        {
            // Assign the loaded image to the target UI Image component
            centerImage.sprite = loadedImage;
        }
        else
        {
            Debug.LogError("Failed to load image at path: " + imagePath);
        }
    }

    void LoadAndAssignPauseImage()
    {
        string imagePath = "Audio Player Assets/" + PlayerPrefs.GetString("mokuID") + "/" + PlayerPrefs.GetString("mokuID") + "_Pause";
        // Load the image from the Resources folder
        Sprite loadedImage = Resources.Load<Sprite>(imagePath);

        // Check if the image is loaded successfully
        if (loadedImage != null)
        {
            // Assign the loaded image to the target UI Image component
            pauseImage.sprite = loadedImage;
        }
        else
        {
            Debug.LogError("Failed to load image at path: " + imagePath);
        }
    }
}
