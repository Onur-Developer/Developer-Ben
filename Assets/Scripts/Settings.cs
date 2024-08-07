using UnityEngine;
using UnityEngine.Audio;


public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private ComminationBen settingsBen;


    public void ChangeVolume(string name, string comingValue)
    {
        int equalsSignIndex = comingValue.IndexOf('=');
        int value = 0;
        
        if (equalsSignIndex != -1)
        {
            string afterEquals = comingValue.Substring(equalsSignIndex + 1);
            
            afterEquals = afterEquals.Trim();
            if (int.TryParse(afterEquals, out int intValue))
                value = intValue;
            else
                return;
            if (value < 0)
                return;
        }
        else
        {
            return;
        }

        string VolumeName = name == "set.MusicVolume" ? "MusicVolume" : "SFXVolume";
        float setValue = value - 80f;
        setValue = Mathf.Clamp(setValue, -80f, 20f);
        audioMixer.SetFloat(VolumeName, setValue);
        float parameterValue;
        float parameterValue2;
        if (audioMixer.GetFloat("MusicVolume", out parameterValue))
        {
            settingsBen.writeValues[3] = "Music Volume=" + (parameterValue + 80f).ToString("F0");
        }

        if (audioMixer.GetFloat("SFXVolume", out parameterValue2))
        {
            settingsBen.writeValues[4] = "SFX Volume=" + (parameterValue2 + 80f).ToString("F0");
        }
    }
}