using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioManager : MonoBehaviour
{
    public static MainAudioManager Instance { get; private set; } = null;

    public AudioSource bgmSource;
    public AudioSource musicSource;
    public AudioSource[] sfxSource;

    private bool IsPlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            bgmSource.volume = PlayerPrefs.GetFloat("BGM", 0.5f);
        }
        else
        {
            musicSource.volume = PlayerPrefs.GetFloat("BGM", 0.5f);
        }

        for (int i = 0; i < sfxSource.Length; ++i)
        {
            if (sfxSource[i].isPlaying == false)
            {
                sfxSource[i].volume = PlayerPrefs.GetFloat("SFX", 0.5f);
            }
        }
    }

    public void PlayBGM(string name)
    {
        AudioClip bgmClip = Resources.Load<AudioClip>("Music/BGM/" + name);
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.volume = PlayerPrefs.GetFloat("BGM", 0.5f);
            bgmSource.Play();
        }
        else
        {
            Debug.LogError("BGM이 나오지 않습니다");
        }
    }

    public void PlayMusicBGM(string Level)
    {
        IsPlaying = true;

        AudioClip bgmClip = Resources.Load<AudioClip>("Music/BGM/" + Level);

        if(bgmClip != null) 
        {
            bgmSource.volume = 0;

            musicSource.volume = PlayerPrefs.GetFloat("BGM", 0.5f);
            musicSource.clip = bgmClip;
            musicSource.Play();
        }
    }

    public void StopMusicBGM()
    {
        IsPlaying = false;

        if(musicSource != null)
        { 
            musicSource.Stop();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string name)
    {
        AudioClip sfxClip = Resources.Load<AudioClip>("Music/SFX/" + name);

        if (sfxClip != null)
        {
            sfxSource[0].clip = sfxClip;
            sfxSource[0].volume = PlayerPrefs.GetFloat("SFX", 0.6f);
            sfxSource[0].spatialBlend = 0;
            sfxSource[0].Play();
        }
        else
        {
            Debug.LogError("SFX가 없습니다");
        }
    }

    public void StopSFX()
    {
        for (int i = 0; i < sfxSource.Length; ++i)
        {
            if (sfxSource[i].isPlaying)
            {
                sfxSource[i].Stop();
            }
        }
    }
}
