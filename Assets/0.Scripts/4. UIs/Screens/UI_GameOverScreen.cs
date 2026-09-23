using UnityEngine;

public class UI_GameOverScreen : UI_ScreenBase
{
    public GameObject DeadScreen;
    public StatModule stat;

    void Start()
    {
        stat.OnDead += OpenDeadScreen;
    }

    void OpenDeadScreen()
    {
        DeadScreen.SetActive(true);
    }
}
