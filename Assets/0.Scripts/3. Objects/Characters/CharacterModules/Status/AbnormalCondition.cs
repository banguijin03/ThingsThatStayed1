using UnityEngine;

public class AbnormalCondition : MonoBehaviour
{
    public bool IsPoison = false;
    public bool IsRunning = false;
    public bool IsBurn = false;
    public bool IsCold = false;

    void Poison()
    {
        IsPoison = true;
    }

    void Running()
    {
        IsRunning = true;
    }

    void Burn()
    {
        IsBurn = true;
    }

    void Cold()
    {
        IsCold = true;
    }
}