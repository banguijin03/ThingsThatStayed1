using System;
using Unity.VisualScripting;
using UnityEngine;

public class UI_TargetHoverInfo : OpenableUIBase
{
    [SerializeField] Vector2 shiftedPosition;//위치

    [SerializeField] TMPro.TextMeshProUGUI nameText;//텍스트
    [SerializeField] TMPro.TextMeshProUGUI toolTipText;//텍스트

    [SerializeField] UnityEngine.UI.Image itemIcon;//이미지

    CharacterBase target;
    //초기화
    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseHover += HoverInfoChange;
        //마우스를 따라 가는 경우
        InputManager.OnMouseMove += MoveToMouse;
    }

    //해제
    public override void Unregistration(UIManager manager)
    {
        base.Unregistration(manager);
        InputManager.OnMouseHover -= HoverInfoChange;
        InputManager.OnMouseMove -= MoveToMouse;
    }

    void HoverInfoChange(GameObject newTarget, GameObject oldTarget)
    {
        CharacterBase ascharacter = newTarget?.GetComponent<CharacterBase>();
        if (ascharacter)
        {
            nameText.SetText(newTarget.name);
            /*HealthBar.value = ascharacter.GetModule<heatPointModule>().percent;*/
            Open();
        }
        else Close();
    }
    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        transform.position = screenPosition + shiftedPosition;
    }

}
