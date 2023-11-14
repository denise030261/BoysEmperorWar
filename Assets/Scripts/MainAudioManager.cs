using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioManager : MonoBehaviour
{
    public static MainAudioManager Instance { get; private set; } = null;

    public AudioSource bgmSource;
    public AudioSource[] sfxSource;

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
        bgmSource.volume = PlayerPrefs.GetFloat("BGM", 0.5f);
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

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string name)
    {
        AudioClip sfxClip = Resources.Load<AudioClip>("Music/SFX/" + name);

        if (sfxClip != null)
        {
            for (int i = 0; i < sfxSource.Length; ++i)
            {
                Debug.Log("SFX 시작");
                sfxSource[i].clip = sfxClip;
                sfxSource[i].volume = PlayerPrefs.GetFloat("SFX", 0.6f);
                sfxSource[i].spatialBlend = 0;
                sfxSource[i].Play();
                return;
            }
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
