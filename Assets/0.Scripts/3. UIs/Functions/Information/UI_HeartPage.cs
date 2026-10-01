using UnityEngine;

public class UI_HeartPage : UIBase
{
    [SerializeField] GameObject heartPage;

    public void Initialize(StatModule statModule)
    {
        statModule.OnHPChanged += OnHPChanged;
    }

    void OnHPChanged(int previousHP, int currentHP)
    {
        heartPage.SetActive(true);
    }

    private void OnDestroy() { }
}