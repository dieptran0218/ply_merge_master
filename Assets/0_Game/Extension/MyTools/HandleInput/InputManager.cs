using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : SingletonMonoBehaviour<InputManager>
{

    public HandleInput input;
    public INPUT_STATE inputState;
    public delegate void InputEvent();

    public event InputEvent OnStartDrag;

    public delegate void InputEvent2(Vector3 position);

    public event InputEvent2 OnTouch;
    public event InputEvent2 OnTap;
    public event InputEvent2 OnDraging;
    public event InputEvent2 OnFinishDrag;

    public float dragThreshold;

    float dragThresholdSqr;
    Vector3 touchPos;

    public bool canTouch;

    public override void Awake()
    {
        base.Awake();
        dragThresholdSqr = dragThreshold * dragThreshold;
    }

    void Update()
    {
        if (!canTouch) return;
        if (inputState == INPUT_STATE.FREE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchPos = Input.mousePosition;
                inputState = INPUT_STATE.TOUCHING;
                OnTouch?.Invoke(Input.mousePosition);
            }
        }
        else if (inputState == INPUT_STATE.TOUCHING)
        {
            if (Vector3.SqrMagnitude(Input.mousePosition - touchPos) > dragThresholdSqr)
            {
                inputState = INPUT_STATE.DRAG;                
                OnStartDrag?.Invoke();
            }
        }else if(inputState == INPUT_STATE.DRAG)
        {
            OnDraging?.Invoke(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (inputState == INPUT_STATE.TOUCHING)
            {
                OnTap?.Invoke(Input.mousePosition);
            }
            else if (inputState == INPUT_STATE.DRAG)
            {
                OnFinishDrag?.Invoke(Input.mousePosition);
            }
            inputState = INPUT_STATE.FREE;
        }
    }


    void OnDestroy()
    {
        OnTouch = null;
        OnStartDrag = null;
        OnDraging = null;
        OnFinishDrag = null;
        OnTap = null;
    }
    public enum INPUT_STATE
    {
        FREE,
        TOUCHING,
        DRAG
    }
}
