using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour, IMenuInitialize
{
    public AudioMixer audioMixer;

    [System.Serializable]
    public class VolumeGroup
    {
        public string name;
        public Slider slider;
    }

    [System.Serializable]
    public class VolumeGroups
    {
        public VolumeGroup masterVolume;
        public VolumeGroup soundEffectsVolume;
        public VolumeGroup musicVolume;
    }

    public VolumeGroups volumeGroups;

    public void Initialize()
    {
        SetVolumeSliders();
    }

    // Sets all the volume sliders
    void SetVolumeSliders()
    {
        SetVolumeSlider(volumeGroups.masterVolume);
        SetVolumeSlider(volumeGroups.soundEffectsVolume);
        SetVolumeSlider(volumeGroups.musicVolume);
    }

    // Sets the volume sliders.
    void SetVolumeSlider(VolumeGroup soundGroup)
    {
        var value = PlayerPrefs.GetFloat(soundGroup.name);
        SetVolume(value, soundGroup.name, false);
        soundGroup.slider.value = value;
    }

    // Set the volume on the game and in the player prefs
    void SetVolume(float volume, string soundGroup, bool save = true)
    {
        audioMixer.SetFloat(soundGroup, volume);
        if (save)
        {
            PlayerPrefs.SetFloat(soundGroup, volume);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume(volume, volumeGroups.masterVolume.name);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        SetVolume(volume, volumeGroups.soundEffectsVolume.name);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(volume, volumeGroups.musicVolume.name);
    }
}
