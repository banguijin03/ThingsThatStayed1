using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public delegate void InitializeEvent();
public delegate void UpdateEvent(float deltaTime);
public delegate void DestroyEvent();

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    UIManager _ui;
    public UIManager UI => _ui;

    DBManager _db;
    public DBManager DB => _db;

    DataManager _data;
    public DataManager Data => _data;

    ObjectManager _objectM;
    public ObjectManager ObjectM => _objectM;

    SaveManager _save;
    public SaveManager Save => _save;

    SettingManager _setting;
    public SettingManager Setting => _setting;

    LanguageManager _language;
    public LanguageManager Language => _language;

    AudioManager _audio;
    public AudioManager Audio => _audio;

    CameraManager _camera;
    public CameraManager Camera => _camera;

    InputManager _input;
    public InputManager Input => _input;

    IEnumerator initializing; 

    public static event InitializeEvent OnInitializeManager;
    public static event InitializeEvent OnInitializeController;
    public static event InitializeEvent OnInitializeCharacter;
    public static event InitializeEvent OnInitializeObject;

    public static event UpdateEvent OnUpdateManager;
    public static event UpdateEvent OnUpdateController;
    public static event UpdateEvent OnUpdateCharacter;
    public static event UpdateEvent OnUpdateObject;

    public static event UpdateEvent OnPhysicsCharacter;
    public static event UpdateEvent OnPhysicsObject;

    public static event DestroyEvent OnDestroyManager;
    public static event DestroyEvent OnDestroyController;
    public static event DestroyEvent OnDestroyCharacter;
    public static event DestroyEvent OnDestroyObject;

    [SerializeField] UIType startScreen = UIType.Title;

    public static bool is2D = true;
    bool isLoading = true;
    bool isPlaying = true;

    void Awake()
    {
        if (Instance == null) 
        {
            _instance = this;
        }
        else 
        {
            Destroy(this);
            return;
        }
        initializing = InitializeManagers();

        StartCoroutine(initializing);

    }

    void OnDestroy() 
    {
        if (initializing != null) StopCoroutine(initializing);
        DeleteManagers(); 
    }
    IEnumerator InitializeManagers()
    {
        int totalLoadCount = 0;
        totalLoadCount += CreateManager(ref _ui).LoadCount;
        totalLoadCount += CreateManager(ref _db).LoadCount;
        totalLoadCount += CreateManager(ref _data).LoadCount;
        totalLoadCount += CreateManager(ref _objectM).LoadCount;
        totalLoadCount += CreateManager(ref _save).LoadCount;
        totalLoadCount += CreateManager(ref _setting).LoadCount;
        totalLoadCount += CreateManager(ref _language).LoadCount;
        totalLoadCount += CreateManager(ref _audio).LoadCount;
        totalLoadCount += CreateManager(ref _camera).LoadCount;
        totalLoadCount += CreateManager(ref _input).LoadCount;

        yield return UI.Initialize(this);
        UIBase loadingUI = UIManager.ClaimOpenScreen(UIType.Loading); 
        IProgress<int> loadingProgress = loadingUI as IProgress<int>;

        loadingProgress?.Set(0, totalLoadCount);
        yield return DB.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Data.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return ObjectM.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return UI.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Save.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Setting.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Language.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Audio.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Camera.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Input.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return null;

        loadingProgress.SetComplete(startScreen, ScreenChangeType.ScreenChanger);

        isLoading = false;
    }

    void DeleteManagers()
    {
        //�����Է�	InputManager
        Input?.Disconnect();
        //������Ʈ	ObjectManager
        ObjectM?.Disconnect();
        //�����		AudioManager
        Audio?.Disconnect();
        //���		LanguageManager
        Language?.Disconnect();
        //����		SettingManager
        Setting?.Disconnect();

        //���̺�		SaveManager
        Save?.Disconnect();
        //ī�޶�		CameraManager
        Camera?.Disconnect();
        //UI		UIManager
        UI?.Disconnect();
        //���������� DataManager
        Data?.Disconnect();
        //�����ͺ��̽� DBManager
        DB?.Disconnect();
    }
    ManagerType CreateManager<ManagerType>(ref ManagerType targetVariable) where ManagerType : ManagerBase
    {
        if (targetVariable == null)
        {
            targetVariable = this.TryAddComponent<ManagerType>();
        }

        return targetVariable;
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public static void Pause()
    {
        Instance.isPlaying = false;
    }

    public static void Unpause()
    {
        Instance.isPlaying = true;
    }

    void InvokeInitializeEvent(ref InitializeEvent OriginEvent)
    {
        if (OriginEvent != null) 
        {
            InitializeEvent CurrentEvent = OriginEvent;
            OriginEvent = null;
            CurrentEvent.Invoke(); 
        }
    }
    void InvokeDestroyEvent(ref DestroyEvent OriginEvent)
    {
        if (OriginEvent != null) 
        {
            DestroyEvent CurrentEvent = OriginEvent; 
            OriginEvent = null; 
            CurrentEvent.Invoke(); 
        }
    }
    void Update()
    {
        if (isLoading) return;

        //�ʱ�ȭ
        //�Ŵ����� �ʱ�ȭ�Ѵ�
        InvokeInitializeEvent(ref OnInitializeManager);
        //ĳ���͸� �ʱ�ȭ�Ѵ�
        InvokeInitializeEvent(ref OnInitializeCharacter);
        //��Ʈ�ѷ��� �ʱ�ȭ�Ѵ� => ĳ���Ͱ� �ִ� ���¿��� ���ư��� �ϴϱ�!
        InvokeInitializeEvent(ref OnInitializeController);
        //������Ʈ�� �ʱ�ȭ�Ѵ�
        InvokeInitializeEvent(ref OnInitializeObject);

        if (isPlaying)
        {
            //������ ���̿� �� �ʰ� ��������?
            float deltaTime = Time.deltaTime;
            //�Ŵ����� ������Ʈ �ϴ� ���
            OnUpdateManager?.Invoke(deltaTime);
            //��Ʈ�ѷ��� ������Ʈ�Ѵ� => ���� �Ǵ��ϰ�
            OnUpdateController?.Invoke(deltaTime);
            //ĳ���͸� ������Ʈ�Ѵ� => ĳ���Ͱ� �����ϰ�
            OnUpdateCharacter?.Invoke(deltaTime);
            //������Ʈ�� ������Ʈ�Ѵ� => ������Ʈ ����
            OnUpdateObject?.Invoke(deltaTime);
        }

        //������Ʈ�� �����Ѵ�
        InvokeDestroyEvent(ref OnDestroyObject);
        //��Ʈ�ѷ��� �����Ѵ�
        InvokeDestroyEvent(ref OnDestroyController);
        //ĳ���͸� �����Ѵ�
        InvokeDestroyEvent(ref OnDestroyCharacter);
        //�Ŵ����� �����Ѵ�
        InvokeDestroyEvent(ref OnDestroyManager);
    }

    void FixedUpdate()
    {
        if (isLoading || !isPlaying) return;

        float deltaTime = Time.fixedDeltaTime;

        OnPhysicsCharacter?.Invoke(deltaTime);
        OnPhysicsObject?.Invoke(deltaTime);
    }
}