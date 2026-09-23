using System.Collections;
using UnityEngine;

public class SettingManager : ManagerBase
{
	protected override IEnumerator OnConnected(GameManager newManager)
	{
		Screen.autorotateToLandscapeLeft		= true;		//카메라가 왼쪽
		Screen.autorotateToLandscapeRight		= true;		//카메라가 오른쪽
		Screen.autorotateToPortrait				= false;	//카메라가 위쪽
		Screen.autorotateToPortraitUpsideDown	= false;    //카메라가 아래쪽
		//Screen.orientation = ScreenOrientation.LandscapeLeft; 방향 고정

		Screen.sleepTimeout = SleepTimeout.NeverSleep;		//터치안해도 화면 안꺼짐
		//Screen.sleepTimeout = SleepTimeout.SystemSetting; 시스템 설정에 따라 꺼짐
		yield return null;
	}

	protected override void OnDisconnected()
	{

	}
}
