using System.Collections.Generic;
using UnityEngine;

public class InteractionCondition : MonoBehaviour
{
    [Header("상호작용 범위")]
    [SerializeField] float interactionRange = 1.38f;

    [Header("방향 조건")]
    [SerializeField] float facingThreshold = 0.2f;

    [Header("대상 선정 가중치")]
    [SerializeField] float distanceWeight = 0.4f;
    [SerializeField] float facingWeight = 0.6f;

    static readonly HashSet<InteractionCondition> conditions = new();
    public static InteractionCondition CurrentTarget { get; private set; }

    PlayerController playerCharacter;

    private void OnEnable()
    {
        conditions.Add(this);
    }

    private void OnDisable()
    {
        conditions.Remove(this);

        if (CurrentTarget == this) CurrentTarget = null;
    }

    public static InteractionCondition FindBestTarget()
    {
        CurrentTarget = null;

        if (GameManager.Instance == null) return null;

        if (GameManager.Instance.Player == null) return null;

        PlayerController playerCharacter = GameManager.Instance.Player.GetComponent<PlayerController>();

        if (playerCharacter == null) return null;

        Vector2 playerPosition = playerCharacter.Character.transform.position;
        Vector2 playerDirection = playerCharacter.Character.LookRotation.normalized;

        float bestScore = float.MinValue;
        InteractionCondition bestTarget = null;

        foreach (InteractionCondition condition in conditions)
        {
            if (condition == null) continue;

            Vector2 targetPosition = condition.transform.position;
            float distance = Vector2.Distance(playerPosition, targetPosition);

            if (distance > condition.interactionRange) continue;

            Vector2 directionToTarget = (targetPosition - playerPosition).normalized;
            float dot = Vector2.Dot(playerDirection, directionToTarget);

            if (dot < condition.facingThreshold) continue;

            float distanceScore = 1f - Mathf.Clamp01(distance / condition.interactionRange);
            float facingScore = Mathf.InverseLerp(condition.facingThreshold, 1f, dot);
            float score = distanceScore * condition.distanceWeight + facingScore * condition.facingWeight;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = condition;
            }
        }

        CurrentTarget = bestTarget;
        return CurrentTarget;
    }

    public bool CanInteract()
    {
        if (GameManager.Instance == null) return false;

        if (GameManager.Instance.Player == null)  return false;

        playerCharacter = GameManager.Instance.Player.GetComponent<PlayerController>();

        if (playerCharacter == null) return false;

        Vector2 playerPosition = playerCharacter.Character.transform.position;
        Vector2 targetPosition = transform.position;
        float distance = Vector2.Distance(playerPosition, targetPosition);

        if (distance > interactionRange) return false;

        Vector2 directionToTarget = (targetPosition - playerPosition).normalized;
        Vector2 playerDirection = playerCharacter.Character.LookRotation.normalized;
        float dot = Vector2.Dot(playerDirection, directionToTarget);

        if (dot < facingThreshold) return false;

        return true;
    }
}