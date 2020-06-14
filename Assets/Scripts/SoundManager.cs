using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            return instance ? instance : (instance = new GameObject("SoundManagerObject").AddComponent<SoundManager>());
        }
    }

    private void Awake()
    {
        //instance = (new GameObject("SoundManagerObject")).AddComponent<SoundManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    public enum Sound
    {
        BACKGROUND_MENU = 101,

        BACKGROUND_PREGAME = 201,
        BOAT_TO_WATER = 202,
        BOAT_OUT_WATER = 203,

        BACKGROUND_INGAME = 301,
        BOOM_HIT_WATER = 302,
        BOOM_MISS_WATER = 303,

        BACKGROUND_OCEAN = 401,
    }

    public enum SoundType
    {
        MUSIC = 1,
        SFX = 2,
    }

    public void PlaySound(Sound sound, bool loop = false)
    {
        GameObject soundObj = new GameObject("Sound");
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        SoundAudioClip soundAudioClip = GetSoundAudioClip(sound);
        // get volume
        float masterVolume = SettingsManager.Instance.MasterVolume;
        float musicVolume = SettingsManager.Instance.MusicVolume;
        float sfxVolume = SettingsManager.Instance.SFXVolume;
        float volume;
        if (soundAudioClip.soundType == SoundType.MUSIC)
        {
            volume = masterVolume * musicVolume / 100.0f;
            audioSource.loop = true;
        }
        else
        {
            volume = masterVolume * sfxVolume / 100.0f;
        }
        audioSource.clip = soundAudioClip.audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.Play();
        if (!loop)
        {
            StartCoroutine(DelayDestroyAudio(audioSource));
        }
    }

    IEnumerator DelayDestroyAudio(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource.gameObject);
    }

    private static SoundAudioClip GetSoundAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip;
            }
        }
        return null;
    }
}
