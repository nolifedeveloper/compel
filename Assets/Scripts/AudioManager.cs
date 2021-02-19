using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class AudioManager : MonoBehaviour
{
    public Slider AUDIOSLIDER;
    public List<SoundSource> ALL_SOUND_SOURCES;
    public static AudioManager instance;
    
    public static float cachedAudioLevel;
    private const float minimumSoundLevel = 0;

    private string PATH = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\nld\compel\";
    private string fullPath => $"{PATH} audiolvl.nld";



    private void Awake()
    {
        instance = this;
        ReadAudioLevel();
    }
    
    private void ReadAudioLevel()
    {
        if (!Directory.Exists(PATH)) { Directory.CreateDirectory(PATH); }

        if (!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, "1");
            cachedAudioLevel = 1;
        }
        else if(File.Exists(fullPath))
        {
            cachedAudioLevel = Mathf.Clamp((float)Convert.ToDouble(File.ReadAllText(fullPath)),0,1);
        }

        if(AUDIOSLIDER != null)
        {
            AUDIOSLIDER.value = cachedAudioLevel;
        }
    }

    private void SetAudioLevelOnAll(float audioT)
    {
        for(int i = 0; i < ALL_SOUND_SOURCES.Count; i++)
        {
           SetAudioSourceLevel(audioT,ALL_SOUND_SOURCES[i]);
        }
    }

    public void SetAudioSourceLevel(float t,SoundSource ss)
    {
        float toSet = Mathf.Lerp(minimumSoundLevel, ss.defaultSoundLevel, t);
        ss.audSource.volume = toSet;
    }


    public void ResumeAllAudioSources()
    {
        for(int i = 0; i < ALL_SOUND_SOURCES.Count; i++)
        {
            ALL_SOUND_SOURCES[i].audSource.UnPause();
        }
    }

    public void PauseAllAudioSources()
    {
        for (int i = 0; i < ALL_SOUND_SOURCES.Count; i++)
        {
            ALL_SOUND_SOURCES[i].audSource.Pause();
        }
    }
    
    public void OnMusicSliderValueChanged()
    {
        cachedAudioLevel = AUDIOSLIDER.value;
        File.WriteAllText(fullPath, cachedAudioLevel.ToString());
        SetAudioLevelOnAll(cachedAudioLevel);
    }
    
}
