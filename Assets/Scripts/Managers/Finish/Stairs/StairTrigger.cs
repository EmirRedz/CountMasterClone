using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    [SerializeField] private StairController myStair;
    [SerializeField] private bool isLastStair;
    [SerializeField] private Animator chestAnimator;
    [SerializeField] private Renderer chestRenderer;
    [SerializeField] private GameObject goldGO;

    [SerializeField] private Transform[] confettiSpawnPoints;

    private bool isStairSoundPlayed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Minion"))
        {
            other.GetComponent<MinionController>().FinishStair();
            GameManager.Instance.currentBonusFactor = myStair.myPointValue;
            if (!isStairSoundPlayed)
            {
                AudioManager.Instance.PlaySound2D("Gate");
                isStairSoundPlayed = true;
            }
            if (GameManager.Instance.playerController.AreAllMinionsFinished())
            {
                if (isLastStair)
                {
                    Invoke(nameof(ShowWin), 5);
                }
                else
                {
                    ShowWin();
                }

            }

            if (isLastStair)
            {
                chestRenderer.enabled = true;
                chestAnimator.enabled = true;
                goldGO.SetActive(true);
            }
            
        }
    }

    private void PlayConfetti()
    {
        foreach (Transform confettiSpawnPoint in confettiSpawnPoints)
        {
            GameManager.Instance.PlayConfetti(confettiSpawnPoint.position, confettiSpawnPoint.eulerAngles);
        }
    }
    private void ShowWin()
    {
        GameManager.Instance.ShowWinScreen();
    }
}
