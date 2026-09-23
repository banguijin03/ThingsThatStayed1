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

    DialogueManager _dialogue;
    public DialogueManager Dialogue => _dialogue;

    ScenarioManager _scenario;
    public ScenarioManager Scenario => _scenario;

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
        totalLoadCount += CreateManager(ref _dialogue).LoadCount;
        totalLoadCount += CreateManager(ref _scenario).LoadCount;

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
        yield return Dialogue.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Scenario.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return null;

        loadingProgress.SetComplete(startScreen, ScreenChangeType.ScreenChanger);

        isLoading = false;
    }

    void DeleteManagers()
    {
        Input?.Disconnect();
        ObjectM?.Disconnect();
        Audio?.Disconnect();
        Language?.Disconnect();
        Setting?.Disconnect();
        Save?.Disconnect();
        Camera?.Disconnect();
        UI?.Disconnect();
        Data?.Disconnect();
        DB?.Disconnect();
        Dialogue?.Disconnect();
        Scenario?.Disconnect();
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

        InvokeInitializeEvent(ref OnInitializeManager);
        InvokeInitializeEvent(ref OnInitializeCharacter);
        InvokeInitializeEvent(ref OnInitializeController);
        InvokeInitializeEvent(ref OnInitializeObject);

        if (isPlaying)
        {
            float deltaTime = Time.deltaTime;
            OnUpdateManager?.Invoke(deltaTime);
            OnUpdateController?.Invoke(deltaTime);
            OnUpdateCharacter?.Invoke(deltaTime);
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