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
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }
}