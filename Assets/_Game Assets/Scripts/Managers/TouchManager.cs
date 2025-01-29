using System;
using Lean.Touch;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public bool IsTouching = false;
    public float TouchDistance = 1f;

    public Action<Vector2> OnTouchBegin;
    public Action<Vector3> OnTouchMoveWorld;
    public Action<Vector2> OnTouchMoveScreen;
    public Action<Vector2> OnTouchEnd;
    
    private LeanFinger _activeFinger;

    public TouchManager Initialize()
    {
        return this;
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LeanTouch.OnFingerUp += OnFingerUp;
    }
    
    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
    
    private void OnFingerDown(LeanFinger finger)
    {
        if (_activeFinger is not null)
            return;

        if (finger.IsOverGui && GameSettingsData.Instance.ignoreUITouches)
            return;

        _activeFinger = finger;
        IsTouching = true;
        OnTouchBegin?.Invoke(finger.ScreenPosition);
    }
    
    private void OnFingerUpdate(LeanFinger finger)
    {
        if (finger.IsOverGui && GameSettingsData.Instance.ignoreUITouches)
            return;

        if (finger != _activeFinger)
            return;

        OnTouchMoveWorld?.Invoke(finger.GetWorldPosition(TouchDistance));
        OnTouchMoveScreen?.Invoke(finger.ScreenPosition);
    }

    private void OnFingerUp(LeanFinger finger)
    {
        if (_activeFinger != finger)
            return;

        if (finger.IsOverGui && GameSettingsData.Instance.ignoreUITouches)
            return;

        _activeFinger = null;
        IsTouching = false;

        OnTouchEnd?.Invoke(finger.ScreenPosition);
    }
}