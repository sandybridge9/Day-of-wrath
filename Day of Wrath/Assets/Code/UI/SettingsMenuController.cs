using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMPro.TMP_Dropdown resolutionDropdown;
    List<Resolution> resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions.ToList();

        resolutionDropdown.ClearOptions();

        var currentResolution = resolutions
            .Where(x =>
                x.width == Screen.currentResolution.width
                && x.height == Screen.currentResolution.height)
            .First();

        var currentResolutionIndex = resolutions.IndexOf(currentResolution);

        resolutionDropdown
            .AddOptions(resolutions.Select(x => $"{x.width} x {x.height}").ToList());

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volumeLevel)
    {
        audioMixer.SetFloat("MainMenuVolume", volumeLevel);
    }

    public void SetGraphicsQuality(int graphicsQualityIndex)
    {
        QualitySettings.SetQualityLevel(graphicsQualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
