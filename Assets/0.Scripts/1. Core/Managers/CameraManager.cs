using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : ManagerBase
{
    public Camera MainCamera { get; private set; }

    Transform target;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        SetMainCamera(Camera.main);
        yield return null;
    }

    protected override void OnDisconnected()
    {
        target = null;
    }

    public void SetMainCamera(Camera wantCamera)
    {
        MainCamera = wantCamera;
    }

    public void SetTarget(Transform wantTarget)
    {
        target = wantTarget;
    }

    void LateUpdate()
    {
        if (MainCamera == null) return;
        if (target == null) return;

        Vector3 position = MainCamera.transform.position;

        position.x = target.position.x;
        position.y = target.position.y;

        MainCamera.transform.position = position;
    }

    public void GetRaycastResult(Vector2 screenPosition, List<RaycastResult> outResult)
    {
        EventSystem currentEvent = EventSystem.current;

        if (!currentEvent) return;

        PointerEventData eventData = new(currentEvent);
        eventData.position = screenPosition;

        currentEvent.RaycastAll(eventData, outResult);

        foreach (RaycastResult result in outResult)
        {
            if (result.gameObject == null) continue;
        }
    }
}