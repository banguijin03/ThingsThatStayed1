using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
//using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DBManager : ManagerBase
{
    FirebaseAuth authentication;
    FirebaseUser user;
    DatabaseReference rootDB;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(InitializeFireBase);
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    void InitializeFireBase(Task<DependencyStatus> task)
    {
        if (task.Result == DependencyStatus.Available)
        {
            authentication = FirebaseAuth.DefaultInstance;
            user = authentication.CurrentUser;
            rootDB = FirebaseDatabase.DefaultInstance.RootReference;

            GuestLogin();
            Debug.Log("Firebase Initialized");
        }
        else
        {
            Debug.LogError($"Fail to Initialize Firebase : {task.Exception}");
        }
    }
    public TMPro.TMP_InputField nickNameInput;
    public TMPro.TMP_InputField petNameInput;
    public void MakeUserData()
    {
        WriteData(MakeNewUserInfo(nickNameInput.text), "users", user.UserId, "userInfo");
        //WriteData(MakeNewUserData(petNameInput.text), "users", user.UserId, "userData");
        //WriteData(MakeNewItemData(), "Data", "Item", "ItemData");
        //WriteData(MakeNewMapData(), "Data", "Map");
        //WriteData(MakeNewNPCData(), "Data", "Npc");
    }
    public void MakepetData()
    {
        //WriteData(MakeNewUserInfo(nickNameInput.text), "users", user.UserId, "userInfo");
        WriteData(MakeNewUserData(petNameInput.text), "users", user.UserId, "userData");
        //WriteData(MakeNewItemData(), "Data", "Item", "ItemData");
        //WriteData(MakeNewMapData(), "Data", "Map");
        //WriteData(MakeNewNPCData(), "Data", "Npc");
    }
    public async void GuestLogin()
    {
        if (authentication is null) return;

        if (user is not null)
        {
            Debug.Log($"Already Login : {user.UserId}");

            // UserInfo 읽기
            UserInfo userInfo = await ReadDataAsync<UserInfo>("users", user.UserId, "userInfo");

            if (userInfo != null)
            {
                Debug.Log(".");
            }
            else
            {
                // 데이터가 없으면 생성
                WriteData(MakeNewUserInfo(nickNameInput.text), "users", user.UserId, "userInfo");
                WriteData(MakeNewUserData(petNameInput.text), "users", user.UserId, "userData");
                WriteData(MakeNewItemData(), "Data", "Item", "ItemData");
                WriteData(MakeNewMapData(), "Data", "Map");
                WriteData(MakeNewNPCData(), "Data", "Npc");
            }

            // UserData 읽기
            UserData userData = await ReadDataAsync<UserData>("users", user.UserId, "userData");

            return;
        }

        await authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
    }

    void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
            return;
        }

        user = task.Result.User;
        WriteData(MakeNewUserInfo(nickNameInput.text), "users", user.UserId, "userInfo");
        WriteData(MakeNewUserData(petNameInput.text), "users", user.UserId, "userData");
        WriteData(MakeNewItemData(), "Data", "Item", "ItemData");
        WriteData(MakeNewMapData(), "Data", "Map");
        WriteData(MakeNewNPCData(), "Data", "Npc");
        Debug.Log($"Sign in Succeed : {user.UserId}");
    }

    void OnTaskResult(Task task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }


    public class UserInfo
    {
        public string SteamID;
        public string Nickname;
        public string Status;
        public string Role;
        public double GameVersion;
    }
    UserInfo MakeNewUserInfo(string wantNickname, string steamId = "steamId", string status = "밴", string role = "관리자" ) => new()
    {
        SteamID = steamId,
        Nickname = wantNickname,
        Status = status,
        Role = role,
        GameVersion = 1.0
    };

    public class UserData
    {
        public int Level;
        public double Exp;
        public int Money;
        public float PlayTime;
        public string PetName;
        public string PreviousPetName;
    }


    UserData MakeNewUserData(string petName, int myLevel = 1, double exp = 12.2233, int money = 1000) => new()
    {
        Level       =   myLevel,
        Exp         =   exp,
        Money       =   money,
        PlayTime    =   0,
        PetName = petName,
        PreviousPetName = petName
    };
    public async void ChangePetName()
    {
        UserData data = await ReadDataAsync<UserData>("users", user.UserId, "userData");

        if (data == null)
            return;

        data.PreviousPetName = data.PetName;
        data.PetName = petNameInput.text;

        WriteData(data, "users", user.UserId, "userData");
    }
    public async void RollbackPetName()
    {
        UserData data = await ReadDataAsync<UserData>("users", user.UserId, "userData");

        if (data == null)
            return;

        string current = data.PetName;

        data.PetName = data.PreviousPetName;
        data.PreviousPetName = current;

        WriteData(data, "users", user.UserId, "userData");
    }

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
    ItemData MakeNewItemData() => new()
    {
        ItemID=00,
        ItemName="사과",
        Description="먹을 수 있는 것",
        ItemTpe="Food",
        //ItemIcon= ,
        MaxCount= 99,
        Heal= 10,
        Damage=1
    };

    public class NPCData
    {
        public string NpcName;
        public float HP;
        public float Power;
        public float Speed;
        public String DropItem;
    }
    NPCData MakeNewNPCData() => new()
    {
        NpcName     = "트롤",
        HP          = 100,
        Power       = 10,
        Speed       = 10,
        DropItem    = "사과"
    };
    public class MapData
    {
        public string MapName;
        public string Descriptionl;
        public string SpawnItems;
        public string SpawnMonsters;
        public string Buildables;
    }
    MapData MakeNewMapData() => new()
    {
        MapName="시작의 마을",
        Descriptionl="시작하는 장소",
        SpawnItems ="사과",
        SpawnMonsters ="트롤",
        Buildables ="제작대"
    };
    

    public DatabaseReference GetFinalDirectory(DatabaseReference root, params string[] directory)
    {
        if (directory is null || directory.Length == 0) return root;
        DatabaseReference currentReference = root;
        foreach (string currentChild in directory)
        {
            currentReference = currentReference.Child(currentChild);
        }
        return currentReference;
    }


    public void WriteData(object wantData, params string[] directory)
    {
        if (rootDB is null || wantData is null) return;
        string jsonData = JsonUtility.ToJson(wantData);
        GetFinalDirectory(rootDB, directory).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(OnTaskResult);
    }

    public void WriteData(Dictionary<string, object> changes, params string[] directory)
    {
        if (rootDB is null || changes is null) return;
        GetFinalDirectory(rootDB, directory).UpdateChildrenAsync(changes).ContinueWithOnMainThread(OnTaskResult);
    }

    public IEnumerator ReadDataCorutin(Action<Task<DataSnapshot>> OnReadData, params string[] directory)
    {
        Task<DataSnapshot> readTask = GetFinalDirectory(rootDB, directory).GetValueAsync();
        yield return readTask.WaitForTask();
        OnReadData?.Invoke(readTask);
    }

    public async Task<T> ReadDataAsync<T>(params string[] directory)
    {
        DataSnapshot currentTask = await GetFinalDirectory(rootDB, directory).GetValueAsync();
        if (currentTask is null) return default;
        if (!currentTask.Exists) return default;
        try
        {
            if (currentTask.HasChildren)
            {
                return JsonUtility.FromJson<T>(currentTask.GetRawJsonValue());
            }
            return (T)System.Convert.ChangeType(currentTask.Value, typeof(T));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return default;
        }
    }
}