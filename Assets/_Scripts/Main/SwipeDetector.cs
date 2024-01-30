using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwipeDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public enum Direction { Up, Down, Left, Right }
    public enum InputSource { Touch, Mouse, Both }

    [System.Serializable]
    public class SwipeEvent : UnityEvent<float> { }

    public RectTransform SwipeArea;
    public InputSource inputSource = InputSource.Both;
    public float SwipeMagnitude = 50f;

    public SwipeEvent OnSwipeUp;
    public SwipeEvent OnSwipeDown;
    public SwipeEvent OnSwipeLeft;
    public SwipeEvent OnSwipeRight;

    private bool isPointerDown = false;
    private Vector2 startPointerPosition;
    private Vector2 currentSwipe;

    private void Update()
    {
        if (inputSource == InputSource.Mouse || inputSource == InputSource.Both)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnPointerDown(new PointerEventData(EventSystem.current) { position = Input.mousePosition });
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnPointerUp(new PointerEventData(EventSystem.current) { position = Input.mousePosition });
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsWithinSwipeArea(eventData.position))
        {
            startPointerPosition = eventData.position;
            isPointerDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown && IsWithinSwipeArea(eventData.position))
        {
            currentSwipe = eventData.position - startPointerPosition;
            DetectSwipe();
        }
        isPointerDown = false;
    }

    private void DetectSwipe()
    {
        if (currentSwipe.magnitude < SwipeMagnitude) return; // Minimum swipe distance

        currentSwipe.Normalize();
        float swipeForce = currentSwipe.magnitude;

        if (Mathf.Abs(currentSwipe.x) > Mathf.Abs(currentSwipe.y))
        {
            // Horizontal swipe
            if (currentSwipe.x > 0)
                OnSwipeRight.Invoke(swipeForce);
            else
                OnSwipeLeft.Invoke(swipeForce);
        }
        else
        {
            // Vertical swipe
            if (currentSwipe.y > 0)
                OnSwipeUp.Invoke(swipeForce);
            else
                OnSwipeDown.Invoke(swipeForce);
        }
    }

    private bool IsWithinSwipeArea(Vector2 position)
    {
        if (SwipeArea == null) return true; // If no swipe area is set, allow swipe anywhere
        return RectTransformUtility.RectangleContainsScreenPoint(SwipeArea, position);
    }
}