using UnityEngine;

public class SoundSettings : MonoBehaviour
{
    private static SoundSettings _instance;

    private float _sfxVolume;
    private float _musicVolume;

    public static SoundSettings Instance => _instance;
    public float SFXVolume => _sfxVolume;
    public float MusicVolume => _musicVolume;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }

    public void SetMusicVolume(float volume) 
    {
        _musicVolume = volume;
    }
}
