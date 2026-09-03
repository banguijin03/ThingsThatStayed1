using UnityEngine;

public class UI_InventoryInput : MonoBehaviour
{
    [SerializeField] float inputCooldown = 0.3f;

    float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.unscaledDeltaTime;
    }

    public bool CanInput()
    {
        if (cooldownTimer > 0f) return false;

        cooldownTimer = inputCooldown; 
        return true;
    }
}