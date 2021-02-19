using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    [HideInInspector]public float defaultSoundLevel = 0;
    public AudioSource audSource => this.GetComponent<AudioSource>();

    private void Start()
    {
        InitSource();
    }

    public void InitSource()
    {
        if(defaultSoundLevel == 0)
        {
            defaultSoundLevel = this.audSource.volume;       
            AudioManager.instance.ALL_SOUND_SOURCES.Add(this);
        }
                
        AudioManager.instance.SetAudioSourceLevel(AudioManager.cachedAudioLevel,this);
    }
}
