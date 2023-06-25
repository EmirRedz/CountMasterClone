using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private MinionController targetMinion;
    private EnemySpawner mySpawner;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private float speed;
    [SerializeField] private Animator enemyAnimator;
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    [SerializeField] private SpriteRenderer enemySplatDecal;

    public void Update()
    {
        if (!HasNoTarget())
        {
            var currentPosition = transform.position;
            transform.position = Vector3.MoveTowards(currentPosition,targetMinion.transform.position, speed * Time.deltaTime);
            enemyAnimator.SetBool(IsRunning, true);
        }
        else
        {
            if (mySpawner.playerMinionsInArea.Count <= 0)
            {
                return;
            }
            enemyAnimator.SetBool(IsRunning, false);
            GetTarget(GetClosestTarget());
        }
    }

    
    private void GetTarget(MinionController minionToTarget)
    {
        if (HasNoTarget())
        {
            targetMinion = minionToTarget;
        }
    }

    private bool HasNoTarget()
    {
        return targetMinion == null || !targetMinion.gameObject.activeInHierarchy;
    }

    private void KillEnemy()
    {
        mySpawner.RemoveEnemyToList(this);
        var deathParticle = LeanPool.Spawn(deathParticlePrefab, transform.position, Random.rotation);
        
        var decalPos = new Vector3(transform.position.x, transform.position.y - 0.9f, transform.position.z);
        var splatDecal = LeanPool.Spawn(enemySplatDecal, decalPos, Quaternion.Euler(90, 0, 0));
        
        var color = new Color(splatDecal.color.r, splatDecal.color.g, splatDecal.color.b, 1);
        splatDecal.color = color;
        
        splatDecal.DOFade(0, 5f).OnComplete(() =>
        {
            LeanPool.Despawn(splatDecal.gameObject);
        });
        
        LeanPool.Despawn(deathParticle, 1f);
        LeanPool.Despawn(gameObject,0.03f);
    }

    MinionController GetClosestTarget()
    {
        MinionController tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (MinionController minion in mySpawner.playerMinionsInArea)
        {
            float dist = Vector3.Distance(minion.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = minion;
                minDist = dist;
            }
        }
        return tMin;
    }
    
    public void Init(EnemySpawner spawner)
    {
        mySpawner = spawner;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Minion"))
        {
            var minion = other.GetComponent<MinionController>();
            if (minion == targetMinion)
            {
                targetMinion.KillMinion();
                mySpawner.RemoveMinion(targetMinion);
                KillEnemy();
            }
        }
    }
}
