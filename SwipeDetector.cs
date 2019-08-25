using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    Vector2 startPosition;
    Vector2 lastPosition;
    Vector2 endPosition;

    [SerializeField]
    bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    float minDistanceForSwipe = 20f;

    public static event Action<SwipeData> OnSwipe = delegate { };

    void Update ()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                endPosition = touch.position;
                startPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                lastPosition = touch.position;
                var direction = DetectSwipeDirection ();
                SendSwipe (direction);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                lastPosition = touch.position;
                var direction = DetectSwipeDirection ();
                SendSwipe (direction);
            }
        }
    }

    SwipeDirection DetectSwipeDirection ()
    {
        var direction = SwipeDirection.NotSwipe;
        if (SwipeDistanceCheckMet ())
        {
            if (IsVerticalSwipe ())
            {
                direction = lastPosition.y - endPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
            else
            {
                direction = lastPosition.x - endPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            endPosition = lastPosition;
        }
        return direction;
    }

    bool IsVerticalSwipe ()
    {
        return VerticalMovementDistance () > HorizontalMovementDistance ();
    }

    bool SwipeDistanceCheckMet ()
    {
        return VerticalMovementDistance () > minDistanceForSwipe || HorizontalMovementDistance () > minDistanceForSwipe;
    }

    float VerticalMovementDistance ()
    {
        return Mathf.Abs (lastPosition.y - endPosition.y);
    }

    float HorizontalMovementDistance ()
    {
        return Mathf.Abs (lastPosition.x - endPosition.x);
    }

    void SendSwipe (SwipeDirection direction)
    {
        var swipeData = new SwipeData ()
        {
            Direction = direction,
            StartPosition = startPosition,
            LastPosition = lastPosition,
            EndPosition = endPosition
        };
        OnSwipe (swipeData);
    }
}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 LastPosition;
    public Vector2 EndPosition;
    public SwipeDirection Direction;
}

public enum SwipeDirection
{
    NotSwipe,
    Up,
    Down,
    Left,
    Right
}
