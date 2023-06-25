using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake cinemachineShakeInstance;

    private CinemachineVirtualCamera cinemachineVirtualCam;

    private void Awake()
    {
        cinemachineVirtualCam = GetComponent<CinemachineVirtualCamera>();
        cinemachineShakeInstance = this;
    }

    public void ShakeCamera(float intensity, float timeToShake)
    {
        StopCoroutine("ShakeCameraCoroutine");
        cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        StartCoroutine("ShakeCameraCoroutine", timeToShake);
    }

    IEnumerator ShakeCameraCoroutine(float timeToShake)
    {
        CinemachineBasicMultiChannelPerlin perlin = cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        float decreaseValue = perlin.m_AmplitudeGain / timeToShake;
        var wait = new WaitForEndOfFrame();

        for(float f = 0; f < timeToShake; f += Time.deltaTime)
        {
            perlin.m_AmplitudeGain -= Time.deltaTime * decreaseValue;
            yield return wait;
        }
        perlin.m_AmplitudeGain = 0;
    }
}
