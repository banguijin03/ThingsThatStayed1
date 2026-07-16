using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataManager : ManagerBase
{
	static Dictionary<System.Type, Dictionary<string, Object>> dataDictionary = new();

	event System.Action DisconnectEvent;

	public override int LoadCount
	{
		get
		{
			var task = Addressables.LoadResourceLocationsAsync("Global");
			var result = task.WaitForCompletion();
			int count = result.Count; 
			task.Release();
			return count;
		}
	}

	protected override IEnumerator OnConnected(GameManager newManager)
	{
		UIBase loading = UIManager.ClaimGetUI(UIType.Loading);
		IProgress<int> progressUI = loading as IProgress<int>;
		IStatus<string> statusUI = loading as IStatus<string>;

		int loaded = 0;
		int total = LoadCount;
		string loadString = "Load Data";
		System.Action ProgressOnLoad = () => 
		{
			loaded++;
			progressUI?.AddCurrent(1);
			statusUI?.SetCurrentStatus($"{loadString} ({loaded}/{total})");
		};
		loadString = "Load Game Objects";
		yield return LoadAllFromAssetBundle<GameObject>("Global", ProgressOnLoad).WaitForTask();

		loadString = "Load Pool Requests";
		yield return LoadAllFromAssetBundle<PoolRequest>("Global", ProgressOnLoad).WaitForTask();

        loadString = "Load Item";
        yield return LoadAllFromAssetBundle<ItemContainer>("Global", ProgressOnLoad).WaitForTask();

        yield return null;
	}

	protected override void OnDisconnected()
	{
		DisconnectEvent?.Invoke();
		DisconnectEvent = null;
	}

	bool TryGetFileFromResources<T>(string path, out T result) where T : Object
	{
		result = Resources.Load<T>(path);
		return result != null;
	}

	public static void SaveDataFile<T>(T target) where T : Object
	{
		if (target == null) return;
		Dictionary<string, Object> innerDictionary;

		if(!dataDictionary.TryGetValue(typeof(T), out innerDictionary))
		{
			innerDictionary = new();
			dataDictionary.Add(typeof(T), innerDictionary);
		}

		innerDictionary.TryAdd(target.name.ToLower(), target);
	}

	protected static T GetDataFromDictionary<T>(string fileName) where T : Object
	{
		if (string.IsNullOrEmpty(fileName)) return null;

		fileName = fileName.ToLower();
		if (dataDictionary.TryGetValue(typeof(T), out Dictionary<string, Object> innerDictionary))
		{
			if (innerDictionary.TryGetValue(fileName, out Object result))
			{
				return result as T; 
			}
		}

		return null;
	}

	public static T LoadDataFile<T>(string fileName) where T : Object
	{
		T result = GetDataFromDictionary<T>(fileName);
		if(!result) UIManager.ClaimErrorMessage(SystemMessage.FileNameNotFound(fileName));
		return result;
	}

	public static bool TryLoadDataFile<T>(string fileName, out T result) where T : Object
	{
		result = GetDataFromDictionary<T>(fileName);
		return result;
	}

	public async Task LoadAllFromAssetBundle<T>(string label, System.Action actionForEachLoad) where T : Object
	{
		var finder = Addressables.LoadAssetsAsync<T>(label, (T loaded) => 
		{
			SaveDataFile(loaded); 
			actionForEachLoad();  
		});
		Task result = finder.Task;
		await result;
		DisconnectEvent += () => finder.Release();
	}

	public async void LoadFileFromAssetBundle<T>(string address) where T : Object
	{
		var finder = Addressables.LoadAssetAsync<T>(address);
		await finder.Task; 
		SaveDataFile(finder.Result);
		finder.Release();
	}
}