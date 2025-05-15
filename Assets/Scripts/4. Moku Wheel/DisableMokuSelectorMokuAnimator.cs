using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMokuSelectorMokuAnimator : MonoBehaviour
{
    private RotateOnDrag rotateOnDrag;

    [Header("Audio Trigger")]
    public AudioSource monitoredAudioSource;

    [Header("Data Tracking")]
    public bool markFirstIntroDoneInGameData = false;

    private bool audioStarted = false;
    private bool hasDisabledAfterAudio = false;

    void Start()
    {
        GameObject wheel = GameObject.FindWithTag("Lokahi Wheel");
        if (wheel != null)
        {
            rotateOnDrag = wheel.GetComponent<RotateOnDrag>();
        }
    }

    void Update()
    {
        if (monitoredAudioSource == null || hasDisabledAfterAudio)
            return;

        // Once audio starts playing, begin monitoring for when it ends
        if (!audioStarted && monitoredAudioSource.isPlaying)
        {
            audioStarted = true;
            Debug.Log("STARTED");
        }

        if (audioStarted && !monitoredAudioSource.isPlaying)
        {
            Debug.Log("DONE");
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("IntroTileFinished");
            }
            hasDisabledAfterAudio = true;
        }
    }

    public void DisableAnimator()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        if (rotateOnDrag != null)
            rotateOnDrag.enabled = true;

        if (markFirstIntroDoneInGameData)
        {
            GameDataManager.Instance.UpdateFirstMokuIntroDone(true);
            GameDataManager.Instance.UpdateFFTileIntroDone(true);
        }
    }
}
