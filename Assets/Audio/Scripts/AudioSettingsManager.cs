using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance;

    [Range(0, 10)]
    public int masterVolume = 10;
    [Range(0, 10)]
    public int fXVolume = 10;
    [Range(0, 10)]
    public int musicVolume = 10; 

    void Awake()
    {
        // Singleton
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

    public int MasterVolume => masterVolume;
    public int FXVolume => fXVolume;
    public int MusicVolume => musicVolume;


}
