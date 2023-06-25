using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public void PlayMusic(AudioSource audioSource , float seconds, bool isLoop)
    {
        StartCoroutine(RaiseVolume(audioSource, seconds,isLoop));
    }
    public void StopMusic(AudioSource audioSource, float seconds)
    {
        StartCoroutine(LowerVolume(audioSource, seconds));
    }

    public void SwapTracks(AudioSource audioSourceA, AudioSource audioSourceB,float lowerTime,float riseTime,bool isLoop)
    {
        StartCoroutine(LowerVolume(audioSourceA, lowerTime));

        StartCoroutine(RaiseVolume(audioSourceB, riseTime,isLoop));
        
    }


    IEnumerator RaiseVolume(AudioSource audioSource,float seconds, bool isLoop)
    {
        float elapsedTime = 0;
        audioSource.loop = isLoop;
        audioSource.Play();
        var volume = PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume");
        while(elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, volume, (elapsedTime / seconds));
            yield return new WaitForEndOfFrame();
        }
        audioSource.volume = volume;
    }

    IEnumerator LowerVolume(AudioSource audioSource, float seconds)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume"), 0, (elapsedTime / seconds));
            yield return new WaitForEndOfFrame();
        }
        audioSource.volume = 0;
        audioSource.loop = false;
        audioSource.Stop();
    }
}
