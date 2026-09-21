using UnityEngine;

public class InteractionCondition : MonoBehaviour
{
    [SerializeField] float interactionRange = 5f;
    [SerializeField] float facingThreshold = 0.75f;

    PlayerController playerCharacter;

    private void Awake()
    {
        playerCharacter = FindAnyObjectByType<PlayerController>(
            FindObjectsInactive.Include
        );
    }

    public bool CanInteract()
    {
        if (playerCharacter == null)
            return false;

        Vector2 playerPosition =
            playerCharacter.Character.transform.position;

        Vector2 targetPosition =
            transform.position;

        float distance =
            Vector2.Distance(playerPosition, targetPosition);

        Vector2 directionToTarget =
            (targetPosition - playerPosition).normalized;

        Vector2 playerDirection =
            playerCharacter.Character.LookRotation.normalized;

        float dot =
            Vector2.Dot(playerDirection, directionToTarget);

        if (distance > interactionRange)
            return false;

        if (dot < facingThreshold)
            return false;

        return true;
    }
}