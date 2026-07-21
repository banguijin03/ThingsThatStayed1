using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CharacterSkinCollect : MonoBehaviour
{
    [SerializeField] Image skinRenderer;
    [SerializeField] Image eyeRenderer;
    [SerializeField] Image hairRenderer;
    [SerializeField] Image clothesRenderer;
    [SerializeField] Image beardRenderer;

    public string genderName;
    public int gender = 0;    //2
    public int skinColor = 0; //4
    public int hairStyle = 0; //6
    public int hairCategory;
    public int hairColor = 0; //4
    public int eyeColor = 0;  //4
    public int clothes = 0;   //6
    public int beard = 0;     //5

    public int direction = 0;

    public void SampleCharacterDirection(int type)
    {
        switch ((Direction)type)
        {
            case Direction.Right:
                direction++;

                if (direction > 1)
                    direction = 0;
                break;

            case Direction.Left:
                direction--;

                if (direction < 0)
                    direction = 1;
                break;

        }
    }
    
    //성별은 눈모양이 달라짐
    //피부색은 독립적
    //헤어 스타일은 머리 모양 바뀜
    //이 헤어스타일을 이용해서 머리색 4개중에 변결
    //성별에서 적용한 눈모양을 바탕으로 눈색변경 
    //옷은 독립적
    //beard는 헤어스타일 색에 따라 같은색으로 있다없다만 구분

    public void Plus(int type)
    {
            switch ((Skin)type)
            {
                case Skin.Gender:
                    gender++;

                    if (gender > 1)
                        gender = 0;
                    break;

                case Skin.SkinColor:
                    skinColor++;

                    if (skinColor > 3)  
                        skinColor = 0;
                    break;

                case Skin.HairStyle:
                    hairStyle++;

                    if (hairStyle > 6)   // 헤어스타일이 7개라면
                        hairStyle = 0;
                    break;

                case Skin.HairColor:
                    hairColor++;

                    if (hairColor > 3)   // 머리색 4개
                        hairColor = 0;
                    break;

                case Skin.EyeColor:
                    eyeColor++;

                    if (eyeColor > 3)    // 눈색 4개
                        eyeColor = 0;
                    break;

                case Skin.Clothes:
                    clothes++;

                    if (clothes > 4)     
                        clothes = 0;
                    break;

                case Skin.Beard:
                    beard++;

                    if (beard > 1)
                        beard = 0;
                    break;
            }

            RefreshCharacter();
    }

    public void Minus(int type)
    {
        switch ((Skin)type)
        {
            case Skin.Gender:
                gender--;

                if (gender < 0)
                    gender = 1;
                break;

            case Skin.SkinColor:
                skinColor--;

                if (skinColor < 0)
                    skinColor = 3;     // 피부색 4종 (0~3)
                break;

            case Skin.HairStyle:
                hairStyle--;

                if (hairStyle < 0)
                    hairStyle = 6;     // 헤어스타일 7종 (0~6)
                break;

            case Skin.HairColor:
                hairColor--;

                if (hairColor < 0)
                    hairColor = 3;     // 머리색 4종 (0~3)
                break;

            case Skin.EyeColor:
                eyeColor--;

                if (eyeColor < 0)
                    eyeColor = 3;      // 눈색 4종 (0~3)
                break;

            case Skin.Clothes:
                clothes--;

                if (clothes < 0)
                    clothes = 4;       // 옷이 5종이면 0~4 (개수에 맞게 수정)
                break;

            case Skin.Beard:
                beard--;

                if (beard < 0)
                    beard = 1;         // 수염 On/Off
                break;
        }

        RefreshCharacter();
    }

    public void RefreshCharacter()
    {
        UpdateGender();
        UpdateSkinColor();
        UpdateHair();
        UpdateEyeColor();
        UpdateClothes();
        UpdateBeardOn();
    }

    public void UpdateGender()
    {
        genderName = (gender == 0) ? "MaleEye" : "FemaleEye";
    }

    public void UpdateSkinColor()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>($"Skins/{skinColor + 1}");

        if (sprites.Length > 0)
            skinRenderer.sprite = sprites[0+direction*4];
    }

    public void UpdateHair()
    {
        int hairCategory = hairStyle + 1;

        Sprite[] sprites = Resources.LoadAll<Sprite>(
            $"Hair's/{hairCategory}/{hairColor + 1 }");

        if (sprites.Length > 0)
            hairRenderer.sprite = sprites[0+direction * 4];
    }

    public void UpdateEyeColor()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(
            $"{genderName}/{eyeColor + 1}");

        if (sprites.Length > 0)
            eyeRenderer.sprite = sprites[0 + direction * 4];
    }

    public void UpdateClothes()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(
            $"Clothers/{clothes + 1}");

        if (sprites.Length > 0)
            clothesRenderer.sprite = sprites[0 + direction * 4];
    }

    public void UpdateBeardOn()
    {
        Sprite[] sprites;

        if (beard == 0)
        {
            sprites = Resources.LoadAll<Sprite>("Beard/off");
        }
        else
        {
            sprites = Resources.LoadAll<Sprite>($"Beard/{hairColor + 1}");
        }

        if (sprites.Length > 0)
            beardRenderer.sprite = sprites[0 + direction * 4];
    }
}


