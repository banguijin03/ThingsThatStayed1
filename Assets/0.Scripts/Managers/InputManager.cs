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
    public static event ButtonEvent OnInteraction;


    public static bool IsShift { get; private set; } = false;
    public static bool IsInputLocked { get; private set; }

    public static void SetInputLocked(bool value)
    {
        IsInputLocked = value;
    }

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
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        RefreshGameObjectUnderCursor(mousePosition);

        OnMouseMove?.Invoke(cursorScreenPosition, cursorWorldPosition);
    }

    void RefreshGameObjectUnderCursor(Vector2 screenPosition)
    {
        cursorHitList.Clear();

        GameManager.Instance.Camera.GetRaycastResult(
            screenPosition,
            cursorHitList
        );

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                -Camera.main.transform.position.z
            )
        );

        worldPosition.z = 0;

        GameObject firstObject = null;

        Debug.Log($"===== Raycast °á°ú {cursorHitList.Count}°³ =====");

        foreach (RaycastResult target in cursorHitList)
        {
            if (target.gameObject == null) continue;

            UI_ItemSlotInfo slotInfo =
                target.gameObject.GetComponentInParent<UI_ItemSlotInfo>();

            Debug.Log(
                $"Raycast: {target.gameObject.name} / " +
                $"UI_ItemSlotInfo: {slotInfo} / " +
                $"SortingOrder: {target.sortingOrder}"
            );
        }

        if (cursorHitList.Count > 0 && cursorHitList[0].element != null)
        {
            firstObject = cursorHitList[0].gameObject;
        }

        if (GameManager.is2D)
        {
            foreach (RaycastResult target in cursorHitList)
            {
                if (target.gameObject == null) continue;

                UI_ItemSlotInfo slotInfo =
                    target.gameObject.GetComponentInParent<UI_ItemSlotInfo>();

                if (slotInfo != null)
                {
                    firstObject = target.gameObject;
                    break;
                }
            }

            if (firstObject == null)
            {
                float GetValue(RaycastResult target)
                {
                    return target.sortingOrder + target.sortingLayer * 100000;
                }

                RaycastResult nearest =
                    cursorHitList.GetMaximum<RaycastResult>(GetValue);

                firstObject = nearest.gameObject;
            }
        }
        else
        {
            float GetDistance(RaycastResult target)
            {
                return target.distance;
            }

            RaycastResult nearest =
                cursorHitList.GetMinimum<RaycastResult>(GetDistance);

            firstObject = nearest.gameObject;
            worldPosition = nearest.worldPosition;
        }

        GameObject lastHoverObject = _cursorHoverObject;
        ISelectable lastHoverSelectable = _cursorHoverSelectable;

        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;

        _cursorHoverObject = firstObject;
        _cursorHoverSelectable =
            _cursorHoverObject?.GetComponent<ISelectable>();

        if (lastHoverObject != _cursorHoverObject)
        {
            OnMouseHover?.Invoke(
                _cursorHoverObject,
                lastHoverObject
            );
        }
    }

    public GameObject GetGameObjectUnderCursor()
    {
        if (cursorHitList.Count == 0)
            return null;

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
        InitializeAction("Move", (context) => { if (IsInputAllowed(InputType.Move)) OnMove?.Invoke(GetVector2Value(context)); }, (context) => OnMove?.Invoke(Vector2.zero));

        InitializeAction("MouseLeftButton", (context) => { if (IsInputAllowed(InputType.Mouse)) OnMouseLeftButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition); }, (context) => { if (IsInputAllowed(InputType.Mouse)) OnMouseLeftButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition); });

        InitializeAction("MouseRightButton", (context) => { if (IsInputAllowed(InputType.Mouse)) OnMouseRightButton?.Invoke(true, cursorScreenPosition, cursorWorldPosition); }, (context) => { if (IsInputAllowed(InputType.Mouse)) OnMouseRightButton?.Invoke(false, cursorScreenPosition, cursorWorldPosition); });

        InitializeAction("MouseWheel", (context) => { if (IsInputAllowed(InputType.Mouse)) OnMouseWheel?.Invoke(GetVector2Value(context).y); });

        InitializeAction("SpaceBar", (context) => { if (IsInputAllowed(InputType.Roll)) OnRoll?.Invoke(true); });

        InitializeAction("ShowStatusButton", (context) => { if (IsInputAllowed(InputType.Interaction)) OnShowStatus?.Invoke(true); }, (context) => { if (IsInputAllowed(InputType.Interaction)) OnShowStatus?.Invoke(false); });

        InitializeAction("Interaction", (context) => { if (IsInputAllowed(InputType.Interaction)) OnInteraction?.Invoke(true); }, 
                                        (context) => { if (IsInputAllowed(InputType.Interaction)) OnInteraction?.Invoke(false); });

        InitializeAction("Cancel", (context) => { if (IsInputAllowed(InputType.Cancel)) OnCancel?.Invoke(true); });

        InitializeAction("Inventory", (context) => { if (IsInputAllowed(InputType.Inventory)) OnInventory?.Invoke(true); });

        InitializeAction("AnyKey", (context) => OnAnyKey?.Invoke());

        InitializeAction("Shift", (context) => { if (IsInputAllowed(InputType.Shift)) { IsShift = true; OnShift?.Invoke(true); } else IsShift = false; }, (context) => { IsShift = false; if (IsInputAllowed(InputType.Shift)) OnShift?.Invoke(false); });
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
    bool IsInputAllowed(InputType inputType)
    {
        if (IsInputLocked)
            return false;

        UI_InputBlocker[] blockers =
            FindObjectsByType<UI_InputBlocker>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

        if (blockers.Length == 0)
            return true;

        foreach (UI_InputBlocker blocker in blockers)
        {
            if (!blocker.IsAllowed(inputType))
                return false;
        }

        return true;
    }
}