using UnityEngine;
using System.Collections;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.GAME_ASSETS).GetComponent<GameAssets>();
            }
            return instance;
        }
    }

    public SoundAudioClip[] soundAudioClips;
}
