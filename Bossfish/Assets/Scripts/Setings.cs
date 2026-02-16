
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(HDAdditionalLightData))]
public class Setings : MonoBehaviour
{
    [SerializeField] Volume volume;

    [SerializeField] Toggle toggle;
    [SerializeField] VolumeProfile volumeProfileRTC;
    [SerializeField] VolumeProfile volumeProfileBGS;

    private void Start()
    {
        if (PlayerPrefs.GetString("volumetricCloudsIsOn") == null)
        {
            PlayerPrefs.SetString("volumetricCloudsIsOn", "true");
        }

        toggle.isOn = PlayerPrefs.GetString("volumetricCloudsIsOn") == "true";
    }

    public void ToggleIsChanged()
    {
        if (toggle.isOn)
        {
            volume.profile = volumeProfileRTC;
            PlayerPrefs.SetString("volumetricCloudsIsOn", "true");
        }
        else
        {
            volume.profile = volumeProfileBGS;
            PlayerPrefs.SetString("volumetricCloudsIsOn", "false");
        }
    }
}
