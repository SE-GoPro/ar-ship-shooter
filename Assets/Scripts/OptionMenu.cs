using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioMixer audioMixer;
    public void SetVolumn(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
