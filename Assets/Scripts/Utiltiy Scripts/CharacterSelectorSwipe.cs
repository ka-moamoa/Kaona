using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectorSwipe : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public float maxScale = 1.5f;
    public float minScale = 1f;
    public float swipeDistance = 50f; // Distance to move on swipe
    public float smoothSpeed = 5f; // Speed of smooth movement
    
    public float amountNeededToTriggerSwipe = 100f; // Distance to move on swipe

    private RectTransform[] buttons;
    private ScrollRect scrollRect;
    private Vector2 startPosition;
    private bool isMoving;

    private Animator[] buttonAnimators;

    private int currentIndex = 0; // Variable to keep track of current index

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
        buttons = new RectTransform[gridLayoutGroup.transform.childCount];
        buttonAnimators = new Animator[buttons.Length];

        for (int i = 0; i < gridLayoutGroup.transform.childCount; i++)
        {
            buttons[i] = gridLayoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
            buttonAnimators[i] = buttons[i].GetComponent<Animator>();
        }

        buttonAnimators[0].SetBool("IsMiddle", true);
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("CanMoveCharacter") == 1){
            UpdateButtonAnimations();
            HandleSwipe();
        }
    }

    private void UpdateButtonAnimations()
    {
        float middleX = Screen.width / 2;
        float screenQuarter = Screen.width / 4;

        for (int i = 0; i < buttons.Length; i++)
        {
            float distanceFromMiddle = Mathf.Abs(buttons[i].position.x - middleX);
            bool isMiddle = distanceFromMiddle < screenQuarter; // Adjust the threshold as needed

            // buttonAnimators[i].SetBool("IsMiddle", isMiddle);
        }
    }

private void HandleSwipe()
{
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPosition = touch.position;
                break;
            case TouchPhase.Moved:
                if (!isMoving) // Only consider swipe if not already moving
                {
                    float swipeDelta = touch.position.x - startPosition.x;

                    // Calculate swipe distance relative to viewport width
                    float viewportWidth = scrollRect.viewport.rect.width;
                    float normalizedSwipeDistance = swipeDistance / viewportWidth;

                    if (Mathf.Abs(swipeDelta) > amountNeededToTriggerSwipe * viewportWidth)
                    {
                        float scrollAmount = Mathf.Sign(swipeDelta) * normalizedSwipeDistance * viewportWidth;
                        Vector2 targetPosition = scrollRect.content.anchoredPosition + new Vector2(scrollAmount, 0f);

                        if (targetPosition.x < 2000 && targetPosition.x > -3000)
                        {
                            StartCoroutine(SmoothMove(targetPosition));
                            currentIndex -= (int)Mathf.Sign(swipeDelta);
                            Debug.Log(currentIndex);

                            if (currentIndex > 0)
                            {
                                buttonAnimators[currentIndex-1].SetBool("IsMiddle", false);
                            }

                            if (currentIndex < 6)
                            {
                                buttonAnimators[currentIndex+1].SetBool("IsMiddle", false);
                            }

                            buttonAnimators[currentIndex].SetBool("IsMiddle", true);
                        }

                        Debug.Log("Swipe Direction: " + (Mathf.Sign(swipeDelta) > 0 ? "Right" : "Left"));
                        Debug.Log("Swipe Direction: " + (Mathf.Sign(swipeDelta)));
                        // Debug.Log(scrollRect.content.anchoredPosition);
                    }
                }
                break;
        }
    }
}

    private IEnumerator SmoothMove(Vector2 targetPosition)
    {
        isMoving = true;
        while ((targetPosition - scrollRect.content.anchoredPosition).sqrMagnitude > 0.01f)
        {
            scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, targetPosition, smoothSpeed * Time.deltaTime);
            yield return null;
        }
        scrollRect.content.anchoredPosition = targetPosition;
        isMoving = false;
    }
}
