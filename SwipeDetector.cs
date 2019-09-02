using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    Vector2 _startPosition;
    Vector2 _lastPosition;
    Vector2 _endPosition;

    [SerializeField]
    bool _detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    float _minDistanceForSwipe = 20f;

    public static event System.Action<SwipeData> OnSwipe = delegate { };

    void Update ()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _endPosition = touch.position;
                _startPosition = touch.position;
            }

            if (!_detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                _lastPosition = touch.position;
                var direction = DetectSwipeDirection ();
                SendSwipe (direction);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _lastPosition = touch.position;
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
                direction = _lastPosition.y - _endPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
            else
            {
                direction = _lastPosition.x - _endPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            _endPosition = _lastPosition;
        }
        return direction;
    }

    bool IsVerticalSwipe ()
    {
        return VerticalMovementDistance () > HorizontalMovementDistance ();
    }

    bool SwipeDistanceCheckMet ()
    {
        return VerticalMovementDistance () > _minDistanceForSwipe || HorizontalMovementDistance () > _minDistanceForSwipe;
    }

    float VerticalMovementDistance ()
    {
        return Mathf.Abs (_lastPosition.y - _endPosition.y);
    }

    float HorizontalMovementDistance ()
    {
        return Mathf.Abs (_lastPosition.x - _endPosition.x);
    }

    void SendSwipe (SwipeDirection direction)
    {
        var swipeData = new SwipeData ()
        {
            Direction = direction,
            StartPosition = _startPosition,
            LastPosition = _lastPosition,
            EndPosition = _endPosition
        };
        OnSwipe (swipeData);
    }
}
