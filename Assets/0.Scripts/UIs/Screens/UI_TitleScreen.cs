using Unity.VisualScripting;
using UnityEngine;

public class UI_TitleScreen : UI_ScreenBase
{
    public void Create() => UIManager.ClaimOpenUI(UIType.CharacterCustomization);
}
