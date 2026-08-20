using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);
public delegate void MouseButtonEvent(bool value, Vector2 screenPosition, Vector3 worldPosition);
public delegate void MouseHoverEvent(GameObject newTarget, GameObject oldTarget);
public delegate void ButtonEvent(bool value);
public delegate void VectorEvent(Vector2 value);
public delegate void AxisEvent(float value);

[RequireComponent(typeof(PlayerInput))]
public class InputManager : ManagerBase
{
    public static event MouseButtonEvent OnMouseLeftButton;
    public static event MouseButtonEvent OnMouseRightButton;
    public static event MouseMoveEvent OnMouseMove;
    public static event MouseHoverEvent OnMouseHover;
    public static event AxisEvent OnMouseWheel;

    public static event ButtonEvent OnCancel;
    public static event ButtonEvent OnShowStatus;
    public static event ButtonEvent OnShift;
    public static event ButtonEvent OnInventory;
    public static event ButtonEvent OnRoll;


    public static bool IsShift { get; private set; } = false;
    void ShiftInput(bool value)
    {
        IsShift = value;
        OnShift?.Invoke(value);
    }

    public static event VectorEvent OnMove;
    public static event Action OnAnyKey;

    static ISelectable _cursorHoverSelectable;
    public static ISelectable CursorHoverSelectable => _cursorHoverSelectable;

    static GameObject _cursorHoverObject;
    public static GameObject CursorHoverObject => _cursorHoverObject;

    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();
    List<RaycastResult> cursorHitList = new();

    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        targetInput = GetComponent<PlayerInput>();

        LoadAllActions();
        InitializeAllActions();

        GameManager.OnUpdateManager -= UpdateEvent;
        GameManager.OnUpdateManager += UpdateEvent;
        yield return null;
    }

    protected override void OnDisconnected()
    {
        GameManager.OnUpdateManager -= UpdateEvent;
    }

    public void UpdateEvent(float deltaTime)
    {
        RefreshGameObjectUnderCursor(cursorScreenPosition);
    }

    void RefreshGameObjectUnderCursor(Vector2 screenPosition)
    {
        cursorHitList.Clear();
        GameManager.Instance.Camera.GetRaycastResult(screenPosition, cursorHitList);

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        GameObject firstObject = null;

        if (cursorHitList.Count > 0 && cursorHitList[0].element != null)
        {
            firstObject = cursorHitList[0].gameObject;
        }

        if (GameManager.is2D)
        {
            worldPosition.z = 0;
            float GetValue(RaycastResult target)
            {
                return target.sortingOrder + target.sortingLayer * 100000;
            }
            RaycastResult nearest = cursorHitList.GetMaximum<RaycastResult>(GetValue);
            firstObject = nearest.gameObject;
        }
        else
        {
            float GetDistance(RaycastResult target)
            {
                return target.distance;
            }
            RaycastResult nearest = cursorHitList.GetMinimum<RaycastResult>(GetDistance);
            firstObject = nearest.gameObject;
            worldPosition = nearest.worldPosition;
        }
        GameObject lastHoverObject = _cursorHoverObject;
        ISelectable lastHoverSelectable = _cursorHoverSelectable;

        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;

        _cursorHoverObject = firstObject;
        _cursorHoverSelectable = _cursorHoverObject?.GetComponent<ISelectable>();

        if (lastHoverObject != _cursorHoverObject)
        {
            OnMouseHover?.Invoke(_cursorHoverObject, lastHoverObject);
        }
    }

    public GameObject GetGameObjectUnderCursor()
    {
        if (cursorHitList.Count == 0) return null;
        return cursorHitList[0].gameObject;
    }

    void LoadAllActions()
    {
        foreach (InputAction currentAction in targetInput.actions)
        {
            actionDictionary.TryAdd(currentAction.name, currentAction);
            currentAction.Enable();
        }
    }

    void InitializeAllActions()
    {
        if (actionDictionary == null || actionDictionary.Count == 0) return;

        InitializeAction("CursorPositionChanged", (context) => CursorPositionChanged(GetVector2Value(context)));

        InitializeAction("Move", (context) => OnMove?.Invoke(GetVector2Value(context))
                               , (context) => OnMove?.Invoke(Vector2.zero));

        InitializeAction("MouseLeftButton", (context) => OnMouseLeftButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
                                          , (context) => OnMouseLeftButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("MouseRightButton", (context) => OnMouseRightButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
                                           , (context) => OnMouseRightButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("MouseWheel",       (context) => OnMouseWheel?.Invoke(GetVector2Value(context).y ));

        InitializeAction("SpaceBar",         (context) => OnRoll?.Invoke(true));

        InitializeAction("ShowStatusButton", (context) => OnShowStatus?.Invoke(true)
                                           , (context) => OnShowStatus?.Invoke(false));

        InitializeAction("Cancel", (context) => OnCancel?.Invoke(true));
        InitializeAction("Inventory", (context) => OnInventory?.Invoke(true));
        InitializeAction("AnyKey", (context) => OnAnyKey?.Invoke());

        InitializeAction("Shift", (context) => OnShift?.Invoke(true)
                                , (context) => OnShift?.Invoke(false));

    }

    void InitializeAction(string actionName, Action<InputAction.CallbackContext> actionMethod, Action<InputAction.CallbackContext> cancelMethod = null)
    {
        if (actionDictionary == null) return;
        if (actionDictionary.TryGetValue(actionName, out InputAction currentInput))
        {
            if (actionMethod is not null) currentInput.performed += actionMethod;
            if (cancelMethod is not null) currentInput.canceled += cancelMethod;
        }
    }

    T GetInputValue<T>(InputAction.CallbackContext context) where T : struct
    {
        if (context.valueType != typeof(T)) return default;
        return context.ReadValue<T>();
    }

    Vector2 GetVector2Value(InputAction.CallbackContext context) => GetInputValue<Vector2>(context);

    void CursorPositionChanged(Vector2 screenPosition)
    {
        RefreshGameObjectUnderCursor(screenPosition);
        OnMouseMove?.Invoke(cursorScreenPosition, cursorWorldPosition);
    }
}