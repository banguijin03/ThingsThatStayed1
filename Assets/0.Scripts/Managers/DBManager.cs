using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DBManager : ManagerBase
{
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        // Firebase 임시 비활성화
        Debug.Log("DBManager Connected - Firebase Disabled");
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }


    // =========================
    // User
    // =========================

    public TMPro.TMP_InputField nickNameInput;
    public TMPro.TMP_InputField petNameInput;


    public void MakeUserData()
    {
        Debug.Log("MakeUserData - Firebase Disabled");
    }


    public void MakepetData()
    {
        Debug.Log("MakepetData - Firebase Disabled");
    }


    public void GuestLogin()
    {
        Debug.Log("GuestLogin - Firebase Disabled");
    }


    // =========================
    // User Info
    // =========================

    public class UserInfo
    {
        public string SteamID;
        public string Nickname;
        public string Status;
        public string Role;
        public double GameVersion;
    }


    UserInfo MakeNewUserInfo(
        string wantNickname,
        string steamId = "steamId",
        string status = "밴",
        string role = "관리자")
    {
        return new UserInfo
        {
            SteamID = steamId,
            Nickname = wantNickname,
            Status = status,
            Role = role,
            GameVersion = 1.0
        };
    }


    // =========================
    // User Data
    // =========================

    public class UserData
    {
        public int Level;
        public double Exp;
        public int Money;
        public float PlayTime;
        public string PetName;
        public string PreviousPetName;
    }


    UserData MakeNewUserData(
        string petName,
        int myLevel = 1,
        double exp = 12.2233,
        int money = 1000)
    {
        return new UserData
        {
            Level = myLevel,
            Exp = exp,
            Money = money,
            PlayTime = 0,
            PetName = petName,
            PreviousPetName = petName
        };
    }


    public void ChangePetName()
    {
        Debug.Log("ChangePetName - Firebase Disabled");
    }


    public void RollbackPetName()
    {
        Debug.Log("RollbackPetName - Firebase Disabled");
    }


    // =========================
    // Item Data
    // =========================

    public class ItemData
    {
        public int ItemID;
        public string ItemName;
        public string Description;
        public string ItemTpe;
        public Image ItemIcon;
        public int MaxCount;
        public float Heal;
        public float Damage;
    }


    ItemData MakeNewItemData()
    {
        return new ItemData
        {
            ItemID = 00,
            ItemName = "사과",
            Description = "먹을 수 있는 것",
            ItemTpe = "Food",
            MaxCount = 99,
            Heal = 10,
            Damage = 1
        };
    }


    // =========================
    // NPC Data
    // =========================

    public class NPCData
    {
        public string NpcName;
        public float HP;
        public float Power;
        public float Speed;
        public string DropItem;
    }


    NPCData MakeNewNPCData()
    {
        return new NPCData
        {
            NpcName = "트롤",
            HP = 100,
            Power = 10,
            Speed = 10,
            DropItem = "사과"
        };
    }


    // =========================
    // Map Data
    // =========================

    public class MapData
    {
        public string MapName;
        public string Descriptionl;
        public string SpawnItems;
        public string SpawnMonsters;
        public string Buildables;
    }


    MapData MakeNewMapData()
    {
        return new MapData
        {
            MapName = "시작의 마을",
            Descriptionl = "시작하는 장소",
            SpawnItems = "사과",
            SpawnMonsters = "트롤",
            Buildables = "제작대"
        };
    }
}