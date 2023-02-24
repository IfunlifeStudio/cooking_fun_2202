using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private static AudioController instance = null;
    public static AudioController Instance
    {
        get { return instance; }
    }
    private bool isSfxMute, isMusicMute, isVibrationOff;
    [SerializeField] private AudioSource sfxSource, musicSource;
    public bool Music
    {
        set
        {
            PlayerPrefs.SetInt("music_audio", value ? 1 : 0);
            isMusicMute = !value;
            if (isMusicMute) musicSource.Stop();
            else musicSource.Play();
        }
        get { return PlayerPrefs.GetInt("music_audio", 1) == 1; }
    }
    public bool SFX
    {
        set
        {
            PlayerPrefs.SetInt("sfx_audio", value ? 1 : 0);
            isSfxMute = !value;
        }
        get { return PlayerPrefs.GetInt("sfx_audio", 1) == 1; }
    }
    public bool Vibration
    {
        get { return PlayerPrefs.GetInt("vibration", 1) == 1; }
        set { PlayerPrefs.SetInt("vibration", value ? 1 : 0); isVibrationOff = !value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;   
            ResetAudio();
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ResetAudio()
    {
        isMusicMute = !Music;
        isSfxMute = !SFX;
        isVibrationOff = !Vibration;
    }
    public void PlaySfx(AudioClip clip)
    {
        if (!isSfxMute)
            sfxSource.PlayOneShot(clip);
    }
    public void PlayMusic(AudioClip clip, bool isLoop)
    {
        musicSource.clip = clip;
        musicSource.loop = isLoop;
        if (!isMusicMute)
            musicSource.Play();
    }
    public void Vibrate()
    {
        if (!isVibrationOff)
        {
            MMVibrationManager.Vibrate();
        }

    }
    public void PauseMusic()
    {
        if(musicSource !=null)
            musicSource.Pause();
    }
    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }
    public void MuteMusic()
    {
        sfxSource.mute = true;
        musicSource.mute = true;
    }
    public void UnMuteMusic()
    {
        if (!isSfxMute)
            sfxSource.mute = false;
        if (!isMusicMute)
            musicSource.mute = false;
    }
}
