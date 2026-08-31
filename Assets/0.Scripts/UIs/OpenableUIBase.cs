using UnityEngine;

public class OpenableUIBase : UIBase, IOpenable
{
    [SerializeField] bool pauseGame;
    [SerializeField] bool closeByCancel;

    public bool PauseGame => pauseGame;
    public bool CloseByCancel => closeByCancel;

    public virtual bool IsOpen => gameObject.activeSelf;

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Toggle()
    {
        gameObject.SetActive(!IsOpen);
    }
}