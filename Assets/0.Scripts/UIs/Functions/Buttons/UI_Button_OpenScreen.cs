using Unity.VisualScripting;
using UnityEngine;

public class UI_Button_OpenScreen : MonoBehaviour
{
    [SerializeField] UIType wantType;
    [SerializeField] ScreenChangeType ChangeType;
    public void Open()
    {
        UIManager.ClaimOpenScreen(wantType, ChangeType);
    }
}
