using UnityEngine;

// Check list when adding/modifying setting
// ***
// 1. Add/Update storage key
// 2. Add/Update property name
// 3. Add/Update setter
// 4. Add/Update getter to LoadAll()
// 5. Add/Update Setting UI to call Setter
// 6. Add/Update Setting UI to get value from here

public class SettingsManager : MonoBehaviour
{
    private const string WATER_QUALITY = "WATER_QUALITY";
    private const string DEFAULT_AR_VIEW = "DEFAULT_AR_VIEW";
    private const string MASTER_VOLUME = "MASTER_VOLUME";
    private const string MUSIC_VOLUME = "MUSIC_VOLUME";
    private const string SFX_VOLUME = "SFX_VOLUME";

    private void Awake()
    {
        LoadAll();
    }

    public float WaterQuality;
    public void SetWaterQuality(float value)
    {
        Logger.Log("Settings: SetWaterQuality - " + value);
        PlayerPrefs.SetFloat(WATER_QUALITY, value);
    }

    public bool DefaultARView;
    public void SetDefaultARView(bool value)
    {
        Logger.Log("Settings: SetDefaultARView - " + value.ToString());
        PlayerPrefs.SetInt(DEFAULT_AR_VIEW, value ? 1 : 0);
    }

    public float MasterVolume;
    public void SetMasterVolume(float value)
    {
        Logger.Log("Settings: SetMasterVolume - " + value);
        PlayerPrefs.SetFloat(MASTER_VOLUME, value);
    }

    public float MusicVolume;
    public void SetMusicVolume(float value)
    {
        Logger.Log("Settings: SetMusicVolume - " + value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
    }

    public float SFXVolume;
    public void SetSFXVolume(float value)
    {
        Logger.Log("Settings: SetSFXVolume - " + value);
        PlayerPrefs.SetFloat(SFX_VOLUME, value);
    }

    private void LoadAll()
    {
        Logger.Log("Settings: Loading all settings from PlayerPrefs...");
        WaterQuality = PlayerPrefs.GetFloat(WATER_QUALITY, 3);
        Logger.Log("Settings: WaterQuality - " + WaterQuality);
        DefaultARView = (PlayerPrefs.GetInt(DEFAULT_AR_VIEW, 0) == 1);
        Logger.Log("Settings: DefaultARView - " + DefaultARView);
        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, 10);
        Logger.Log("Settings: MasterVolume - " + MasterVolume);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 10);
        Logger.Log("Settings: MusicVolume - " + MusicVolume);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 10);
        Logger.Log("Settings: SFXVolume - " + SFXVolume);
    }

    private void OnDestroy()
    {
        Logger.Log("Settings: Saving...");
        PlayerPrefs.Save();
    }
}
