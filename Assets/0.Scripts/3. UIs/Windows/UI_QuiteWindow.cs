using UnityEngine;

public class UI_QuiteWindow : UI_ScreenBase
{
    public void Confirm() => GameManager.QuitGame();
    public void Cancel() => UIManager.ClaimCloseUI(UIType.GameQuit);
}
