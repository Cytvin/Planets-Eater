using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField]
    private Button _apply;
    [SerializeField]
    private Button _cancel;
    [SerializeField]
    private Slider _sfxVolume;
    [SerializeField]
    private Slider _musicVolume;

    private float _sfxVolumeValue;
    private float _musicVolumeValue;

    private void Start()
    {
        _apply.onClick.AddListener(OnApply);
        _cancel.onClick.AddListener(OnCancel);

        _sfxVolume.onValueChanged.AddListener(SetSFXVolumeValue);
        _musicVolume.onValueChanged.AddListener(SetMusicVolumeValue);
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        _sfxVolumeValue = SoundSettings.Instance.SFXVolume;
        _musicVolumeValue = SoundSettings.Instance.MusicVolume;
    }

    private void SetSFXVolumeValue(float sfxVolumeValue)
    {
        _sfxVolumeValue = sfxVolumeValue;
    }

    private void SetMusicVolumeValue(float musicVolumeValue)
    {
        _musicVolumeValue = musicVolumeValue;
    }

    private void OnApply()
    {
        SoundSettings.Instance.SetSFXVolume(_sfxVolumeValue);
        SoundSettings.Instance.SetMusicVolume(_musicVolumeValue);

        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
