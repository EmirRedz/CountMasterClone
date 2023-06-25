using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannle { Master,Music,Sfx};
    
    public float masterVolumePercent { get; private set;}
    public float musicVolumePercent { get; private set;}
    public float sfxVolumePercent { get; private set;}

    AudioSource sfx2DSource;

    private AudioLowPassFilter sfxFilter;
    //AudioSource[] musicSources;
    //int activeMusicSourceIndex;

    Transform audioListenerTransform;
    Transform playerTransform;
    Sounds sounds;


    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sounds = GetComponent<Sounds>();
            GameObject newSfxSource = new GameObject("Sfx Source");
            sfx2DSource = newSfxSource.AddComponent<AudioSource>();
            newSfxSource.transform.SetParent(transform);
            sfxFilter = newSfxSource.AddComponent<AudioLowPassFilter>();

            audioListenerTransform = GetComponentInChildren<AudioListener>().transform;

            if(!PlayerPrefs.HasKey("MasterVolume") 
                || !PlayerPrefs.HasKey("MusicVolume") 
                || !PlayerPrefs.HasKey("SfxVolume"))
            {
                PlayerPrefs.SetFloat("MasterVolume", 1);
                PlayerPrefs.SetFloat("MusicVolume", 1);
                PlayerPrefs.SetFloat("SfxVolume", 1);
            }
            masterVolumePercent = PlayerPrefs.GetFloat("MasterVolume");
            musicVolumePercent = PlayerPrefs.GetFloat("MusicVolume");
            sfxVolumePercent = PlayerPrefs.GetFloat("SfxVolume");

        }
        
    }
    
    private void Update()
    {
        if(playerTransform != null)
        {
            audioListenerTransform.position = playerTransform.position;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
         
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
         
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        sfx2DSource.volume = sfxVolumePercent * masterVolumePercent;
        
        if (playerTransform == null)
        {
            if (FindObjectOfType<PlayerController>() != null)
            {
                playerTransform = FindObjectOfType<PlayerController>().transform;
            }
        }
    }

    public void SetVolume(float volumePercent, AudioChannle channle)
    {
        switch (channle)
        {
            case AudioChannle.Master:
                masterVolumePercent = volumePercent;
                break;

            case AudioChannle.Music:
                musicVolumePercent = volumePercent;
                break;

            case AudioChannle.Sfx:
                sfxVolumePercent = volumePercent;
                break;
        }
        PlayerPrefs.SetFloat("MasterVolume", masterVolumePercent);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumePercent);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolumePercent);
        PlayerPrefs.Save();

    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(sounds.GetAudioFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(sounds.GetAudioFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }
    
    /*
    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(MusicCrossfade(fadeDuration));
    }

    IEnumerator MusicCrossfade(float duration)
    {
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent,percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent,0,percent);
            yield return null;
        }
    }
    */
}
