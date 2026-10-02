using System.Collections;
using UnityEngine;

public class AudioManager : ManagerBase
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        yield return null;
    }

    protected override void OnDisconnected()
    {
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;

        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }
}