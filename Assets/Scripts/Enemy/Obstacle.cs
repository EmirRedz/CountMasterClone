using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    private IEnumerator Start()
    {
        Animator animator = GetComponentInParent<Animator>();
        if (animator == null)
        {
            yield break;
        }
        animator.enabled = false;
        yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        animator.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Minion"))
        {
            other.GetComponent<MinionController>().KillMinion();
        }
    }
}
