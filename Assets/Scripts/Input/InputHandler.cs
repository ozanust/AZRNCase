using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public delegate void SwipeAction(Vector2 delta);
    public static event SwipeAction OnSwipe;

    private Vector2 startTouchPosition;

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 endTouchPosition = touch.position;
                Vector2 deltaVector = endTouchPosition - startTouchPosition;
                OnSwipe?.Invoke(deltaVector);
            }
        }
    }

    private void HandleMouseInput()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 endTouchPosition = mousePosition;
            Vector2 deltaVector = endTouchPosition - startTouchPosition;
            OnSwipe?.Invoke(deltaVector);
        }
    }
}