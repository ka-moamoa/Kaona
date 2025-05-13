// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using System.Collections;

// public class CharacterSelectorScrollBehaviour : MonoBehaviour
// {
//     public ScrollRect scrollRect;
//     public RectTransform content;
//     public float snapSpeed = 5f;

//     private bool isDragging = false;

//     private void Start()
//     {
//         EventTrigger trigger = scrollRect.gameObject.AddComponent<EventTrigger>();

//         // Add event trigger for when dragging starts
//         EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
//         beginDragEntry.eventID = EventTriggerType.BeginDrag;
//         beginDragEntry.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data); });
//         trigger.triggers.Add(beginDragEntry);

//         // Add event trigger for when dragging ends
//         EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
//         endDragEntry.eventID = EventTriggerType.EndDrag;
//         endDragEntry.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
//         trigger.triggers.Add(endDragEntry);
//     }

//     private void OnBeginDragDelegate(PointerEventData data)
//     {
//         isDragging = true;
//         Debug.Log("Dragging Started");
//     }

//     private void OnEndDragDelegate(PointerEventData data)
//     {
//         isDragging = false;
//         Debug.Log("Dragging Ended");
//         SnapToClosest();
//     }

//     public void SnapToClosest()
//     {
//         // Calculate center position of viewport
//         float viewportCenter = scrollRect.viewport.rect.width * 0.5f;

//         Debug.Log("viewportCenter: " + viewportCenter);

//         // Find closest button
//         Button closestButton = null;
//         float minDistance = float.MaxValue;
//         foreach (Transform button in content)
//         {
//             float distance = Mathf.Abs(button.position.x - viewportCenter);
//             if (distance < minDistance)
//             {
//                 minDistance = distance;
//                 closestButton = button.GetComponent<Button>();
//             }
//         }

//         Debug.Log("closestButton: " + closestButton);

//         // Calculate target position
//         float targetPosX = closestButton.transform.position.x - viewportCenter;

//         // Smoothly scroll to the target position
//         StartCoroutine(SmoothScroll(targetPosX));
//     }

//     IEnumerator SmoothScroll(float targetX)
//     {
//         while (isDragging || Mathf.Abs(content.anchoredPosition.x - targetX) > 0.1f)
//         {
//             content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, new Vector2(targetX, content.anchoredPosition.y), snapSpeed * Time.deltaTime);
//             yield return null;
//         }
//     }
// }



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelectorScrollBehaviour : MonoBehaviour, IDragHandler
{
    public GridLayoutGroup gridLayoutGroup;
    public float maxScale = 1.5f;
    public float minScale = 1f;

    private RectTransform[] buttons;
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
        buttons = new RectTransform[gridLayoutGroup.transform.childCount];
        for (int i = 0; i < gridLayoutGroup.transform.childCount; i++)
        {
            buttons[i] = gridLayoutGroup.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        UpdateButtonScales();
    }

    private void UpdateButtonScales()
    {
        float middleX = scrollRect.viewport.rect.width / 2;
        for (int i = 0; i < buttons.Length; i++)
        {
            float distanceFromMiddle = Mathf.Abs(buttons[i].position.x - middleX);
            float scaleFactor = Mathf.Clamp01(1 - (distanceFromMiddle / middleX)) * (maxScale - minScale) + minScale;
            buttons[i].localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollRect.horizontal)
        {
            float clampedX = Mathf.Clamp(scrollRect.content.anchoredPosition.x, GetLeftClamp(), GetRightClamp());
            scrollRect.content.anchoredPosition = new Vector2(clampedX, scrollRect.content.anchoredPosition.y);
        }
    }

    private float GetLeftClamp()
    {
        RectTransform leftButton = buttons[0];
        float leftEdge = leftButton.position.x - leftButton.rect.width * 0.5f;
        return leftEdge - scrollRect.viewport.rect.width * 0.5f;
    }

    private float GetRightClamp()
    {
        RectTransform rightButton = buttons[buttons.Length - 1];
        float rightEdge = rightButton.position.x + rightButton.rect.width * 0.5f;
        return rightEdge - scrollRect.viewport.rect.width * 0.5f;
    }
}
