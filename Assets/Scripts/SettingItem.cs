using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingItem : MonoBehaviour
{
    public string StorageKey;
    public float defaultFloatValue = 0;
    public int defaultIntValue = 1;
    public bool isSlider;

    private void Awake()
    {
        if (isSlider)
        {
            gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat(StorageKey, defaultFloatValue);
        }
        else
        {
            gameObject.GetComponent<Toggle>().isOn = (PlayerPrefs.GetInt(StorageKey, defaultIntValue) == 1);
        }
    }
}
