using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMPro.TMP_Dropdown resolutionDropdown;
    List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        var supportedResolutions = Screen.resolutions.ToList();

        foreach (var supportedResolution in supportedResolutions)
        {
            if(!resolutions.Any(x => x.width == supportedResolution.width && x.height == supportedResolution.height))
            {
                resolutions.Add(supportedResolution);
            }
        }

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
