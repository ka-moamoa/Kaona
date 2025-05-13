using UnityEngine;
using System.Linq;
using System; // Add this line for LINQ functionality

public class RotateOnDrag : MonoBehaviour
{
    private Vector3 touchStartPos;
    private bool isDragging = false;
    public float dragRotationSpeed = 5f;

    public float autoRotationSpeed = 5f;

    public float[] targetAngles = { 30f, 90f, 150f, 210f, 270f, 330f, -30f, -90f };

    public Animator startAnim;

    public AudioSource startAudio;

    bool startAnimationPlayed = false;
    bool done = false;

    void Start(){
        Debug.Log("SS[1] value: " + GameDataManager.Instance.gameData.SS[1]);
        // GameDataManager.Instance.UpdateFFData(1, true);
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

        if(startAudio.isPlaying == false && startAnimationPlayed == true && done == false){
            // startAnim.enabled = false;
            
            startAnim.Play("Start Animation",0,0.97f);
            startAnim.speed = 1;
            done = true;
        }
    }

    void StartDragging()
    {
        touchStartPos = Input.mousePosition;
        isDragging = true;
    }

    void StopDragging()
    {
        isDragging = false;
        StartCoroutine(SmoothlyRotateToNearestAngle());
    }

    public void disableStartAnimator(){
        startAnim.enabled = false;
    }

    public void playStartAudio(){
        startAudio.Play();
        Debug.Log("PLAY");

        startAnimationPlayed = true;
    }

    public void tempStopAudio(){
        Debug.Log("PLAY");
        startAudio.Stop();
    }

    //Old Rotation Code

    // void HandleRotation()
    // {
    //     Vector2 dragDelta = Input.mousePosition - touchStartPos;
    //     float rotationDirection = Mathf.Sign(dragDelta.y);
    //     float rotationAmount = -rotationDirection * dragDelta.magnitude * dragRotationSpeed * Time.deltaTime;

    //     float currentRotation = transform.eulerAngles.z;
    //     float clampedRotation = Mathf.Clamp(currentRotation + rotationAmount, Mathf.Min(targetAngles), Mathf.Max(targetAngles));

    //     transform.rotation = Quaternion.Euler(0f, -90f, clampedRotation);
    //     touchStartPos = Input.mousePosition;
    // }

void HandleRotation()
{
    Vector2 dragDelta = Input.mousePosition - touchStartPos;
    float rotationDirection = Mathf.Sign(dragDelta.y);
    float rotationAmount = -rotationDirection * dragDelta.magnitude * dragRotationSpeed * Time.deltaTime;

    float currentRotation = transform.eulerAngles.z;
    float clampedRotation = currentRotation + rotationAmount;

    float minTarget = targetAngles.Min();
    float maxTarget = targetAngles.Max();

    // Smooth wrapping logic for rotation
    if (clampedRotation < minTarget - 180f)
    {
        clampedRotation = Mathf.Lerp(currentRotation, maxTarget, Mathf.InverseLerp(minTarget - 180f, minTarget, clampedRotation));
    }
    else if (clampedRotation > maxTarget + 180f)
    {
        clampedRotation = Mathf.Lerp(currentRotation, minTarget, Mathf.InverseLerp(maxTarget, maxTarget + 180f, clampedRotation));
    }

    transform.rotation = Quaternion.Euler(0f, -90f, clampedRotation);
    touchStartPos = Input.mousePosition;
}



    System.Collections.IEnumerator SmoothlyRotateToNearestAngle()
    {
        float currentAngle = transform.eulerAngles.z;
        float nearestAngle = FindNearestAngle(currentAngle);

        while (!Mathf.Approximately(transform.eulerAngles.z, nearestAngle))
        {
            float step = autoRotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.Euler(0f, -90f, nearestAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            yield return null;
        }
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
