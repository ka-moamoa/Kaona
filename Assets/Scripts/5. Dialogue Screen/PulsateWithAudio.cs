using UnityEngine;

public class PulsateWithAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject targetObject;
    public float scaleMultiplier = 0.5f; // Adjust the pulsation intensity
    public float pulsationSpeed = 1f; // Adjust the speed of the pulsation

    public float scaleIndex = 1.25f;

    private void Start()
    {
        if (audioSource == null)
        {
            // Assuming the audio source is on the same GameObject, you can get it dynamically
            audioSource = GetComponent<AudioSource>();
        }

        if (targetObject == null)
        {
            // If the target object is not set, use the current GameObject
            targetObject = gameObject;
        }
    }

    private void Update()
    {
        // Ensure the audio source is playing
        if (audioSource.isPlaying)
        {
            // Use a sine wave to create a smooth pulsation effect
            float pulsation = Mathf.Sin(Time.time * pulsationSpeed) * scaleMultiplier;
            float scale = scaleIndex + audioSource.volume * pulsation;
            targetObject.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            // Reset the scale when audio is not playing
            targetObject.transform.localScale = Vector3.one;
        }
    }
}
