using UnityEngine;

public class UI_LoadingScreen : UI_ScreenBase
    , IOpenable, IProgress<int>, IStatus<string>
{
    public int Current { get; protected set; }
    public int Max { get; protected set; }

    public float Progress => Max != 0 ? (float)Current / Max : 0.0f;

    public int AddCurrent(int value) => Set(Current + value, Max);
    public int AddMax(int value) => Set(Current, Max + value);

    public UnityEngine.UI.Slider progressBar;
    public TMPro.TextMeshProUGUI progressText;
    public TMPro.TextMeshProUGUI explainText;

    public GameObject layoutOnComplete;
    public GameObject layoutOnLoading;

    UIType targetScreen;
    ScreenChangeType screenChangeType;

    public string SetCurrentStatus(string newText)
    {
        explainText.SetText(newText);
        return newText;
    }

    public void SetComplete(UIType openScreen, ScreenChangeType changeType)
    {
        GameManager.Pause();
        InputManager.OnAnyKey -= ExitLoading;
        InputManager.OnAnyKey += ExitLoading;
        targetScreen = openScreen;
        screenChangeType = changeType;
        layoutOnComplete.SetActive(true);
        layoutOnLoading.SetActive(false);
    }

    public int Set(int newCurrent)
    {
        Current = Mathf.Min(newCurrent, Max);
        progressBar.value = Progress;
        progressText.SetText($"{Progress * 100.0f: 0.00}%");
        return Current;
    }

    public int Set(int newCurrent, int newMax)
    {
        layoutOnComplete.SetActive(false);
        layoutOnLoading.SetActive(true);
        Max = newMax;
        return Set(newCurrent);
    }

    public void ExitLoading()
    {
        InputManager.OnAnyKey -= ExitLoading;
        UIManager.ClaimOpenScreen(targetScreen, screenChangeType);
        GameManager.Unpause();
    }

}