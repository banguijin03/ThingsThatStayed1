using UnityEngine;
//상호작용
public class InteractionModule : CharacterModule
{
    IInteractable currentTarget;

    void OnEnable()
    {
        InputManager.OnMouseLeftButton += OnInteract;
    }

    void OnDisable()
    {
        InputManager.OnMouseLeftButton -= OnInteract;
    }

    void OnInteract(bool value, Vector2 screen, Vector3 world)
    {
        if (!value) return;

        if (currentTarget == null) return;

        if (!currentTarget.IsInteractable(gameObject)) return;

        currentTarget.Interact(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        currentTarget = other.GetComponent<IInteractable>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentTarget)
        {
            currentTarget = null;
        }
    }
}
