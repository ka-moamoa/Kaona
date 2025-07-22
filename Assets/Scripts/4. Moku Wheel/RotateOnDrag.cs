using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using CandyCoded.HapticFeedback;


public class RotateOnDrag : MonoBehaviour
{
    private Vector3 touchStartPos;
    private bool isDragging = false;
    public float dragRotationSpeed = 5f;
    public float autoRotationSpeed = 5f;

    public float[] targetAngles = { 30f, 90f, 150f, 210f, 270f, 330f, -30f, -90f };

    public Animator startAnim;
    public AudioSource startAudio;

    private bool startAnimationPlayed = false;
    private bool done = false;

    private float lastRotationZ;
    private HashSet<float> anglesHitThisDrag = new HashSet<float>();
    private const float threshold = 1f;

    void Start()
    {
        Debug.Log("SS[1] value: " + GameDataManager.Instance.gameData.SS[1]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }

        if (isDragging)
        {
            HandleRotation();
        }
    }

    void StartDragging()
    {
        touchStartPos = Input.mousePosition;
        isDragging = true;
        anglesHitThisDrag.Clear();
        lastRotationZ = transform.eulerAngles.z;
    }

    void StopDragging()
    {
        isDragging = false;
        anglesHitThisDrag.Clear();
        StartCoroutine(SmoothlyRotateToNearestAngle());
    }

    void OnEnable()
    {
        isDragging = false;
    }

    void OnDisable()
    {
        isDragging = false;
    }

    public void disableStartAnimator()
    {
        startAnim.enabled = false;
    }

    void HandleRotation()
    {
        Vector2 dragDelta = Input.mousePosition - touchStartPos;
        float rotationDirection = Mathf.Sign(dragDelta.y);
        float rotationAmount = -rotationDirection * dragDelta.magnitude * dragRotationSpeed * Time.deltaTime;

        float currentRotation = transform.eulerAngles.z;
        float clampedRotation = currentRotation + rotationAmount;

        float minTarget = targetAngles.Min();
        float maxTarget = targetAngles.Max();

        if (clampedRotation < minTarget - 180f)
        {
            clampedRotation = Mathf.Lerp(currentRotation, maxTarget, Mathf.InverseLerp(minTarget - 180f, minTarget, clampedRotation));
        }
        else if (clampedRotation > maxTarget + 180f)
        {
            clampedRotation = Mathf.Lerp(currentRotation, minTarget, Mathf.InverseLerp(maxTarget, maxTarget + 180f, clampedRotation));
        }

        transform.rotation = Quaternion.Euler(0f, -90f, clampedRotation);

        CheckAngleCrossings(lastRotationZ, clampedRotation);
        lastRotationZ = clampedRotation;

        touchStartPos = Input.mousePosition;
    }

    void CheckAngleCrossings(float prevAngle, float currAngle)
    {
        foreach (float target in targetAngles)
        {
            float normalizedPrev = NormalizeAngle(prevAngle);
            float normalizedCurr = NormalizeAngle(currAngle);
            float normalizedTarget = NormalizeAngle(target);

            bool crossed = (normalizedPrev < normalizedTarget && normalizedCurr >= normalizedTarget) ||
                           (normalizedPrev > normalizedTarget && normalizedCurr <= normalizedTarget);

            if (crossed && !anglesHitThisDrag.Contains(target))
            {
                TriggerPointFeedback(target);
                anglesHitThisDrag.Add(target);
            }

            if (Mathf.Abs(normalizedCurr - normalizedTarget) > threshold + 5f)
            {
                anglesHitThisDrag.Remove(target);
            }
        }
    }

    void TriggerPointFeedback(float angle)
    {
        Debug.Log("POINT " + angle);

        HapticFeedback.HeavyFeedback();
        
        // 🔔 Insert your haptic feedback here
        // Example for OVR:
        // OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);

        // Example for XR Toolkit (if using XRBaseController):
        // myController.SendHapticImpulse(0.5f, 0.1f);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    System.Collections.IEnumerator SmoothlyRotateToNearestAngle()
    {
        float currentAngle = transform.eulerAngles.z;
        float nearestAngle = FindNearestAngle(currentAngle);

        Quaternion targetRotation = Quaternion.Euler(0f, -90f, nearestAngle);

        while (!Mathf.Approximately(transform.eulerAngles.z, nearestAngle))
        {
            float step = autoRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            yield return null;
        }

        transform.rotation = targetRotation;

        // 🎯 Trigger feedback on arrival
        TriggerPointFeedback(nearestAngle);
    }

    float FindNearestAngle(float currentAngle)
    {
        float nearestAngle = targetAngles[0];
        float minDifference = Mathf.Abs(targetAngles[0] - currentAngle);

        foreach (float angle in targetAngles)
        {
            float difference = Mathf.Abs(angle - currentAngle);
            if (difference < minDifference)
            {
                minDifference = difference;
                nearestAngle = angle;
            }
        }

        return nearestAngle;
    }
}