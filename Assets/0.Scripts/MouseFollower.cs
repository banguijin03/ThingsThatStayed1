using UnityEngine;

public class MouseFollower : MonoBehaviour, IFunctionable
{
    void Start()
    {
        RegistrationFunctions();
    }

    void OnDestroy()
    {
        UnregistrationFunctions();
    }

    public void RegistrationFunctions()
    {
    }

    public void UnregistrationFunctions()
    {
        InputManager.OnMouseLeftButton -= CreateToMouse;
        InputManager.OnMouseRightButton -= DestroyOnMouse;
    }

    void DestroyOnMouse(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (!value) return;

        ObjectManager.DestroyObject(
            GameManager.Instance.Input.GetGameObjectUnderCursor()
        );
    }

    void CreateToMouse(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        if (value) return;

        GameObject inst =
            ObjectManager.CreateObject("NemoMan", worldPosition);
    }

    void MoveToMouse(bool value, Vector2 screenPosition, Vector3 worldPosition)
    {
        DestroyOnMouse(value, screenPosition, worldPosition);
    }
}