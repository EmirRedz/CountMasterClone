using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinionController : MonoBehaviour
{
    [SerializeField] private Animator minionAnimator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private SpriteRenderer minionSplatDecal;
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int CheerID = Animator.StringToHash("Cheer");

    [SerializeField] float minionRotationAngle;
    [SerializeField] private float rotationSpeed;
    
    private bool isStacked;
    private bool isFinishedStair;
    
    [Header("Fighting Boss")] 
    [SerializeField] private float attackCooldown = 1;
    private float lastAttack;
    
    private bool isFighting;
    private bool isAttacking;
    private BossController boss;
    private static readonly int IsFighting = Animator.StringToHash("IsFighting");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

    private void Update()
    {

        if (isFinishedStair)
        {
            return;
        }
        
        if (isFighting)
        {
            if (isAttacking)
            {
                if (lastAttack <= 0)
                {
                    Debug.Log("Attack");
                    int randomAttack = Random.Range(0, 2);
                    minionAnimator.SetFloat(AttackIndex, randomAttack);
                    boss.TakeDamage();
                    lastAttack = attackCooldown;
                }
                else
                {
                    lastAttack -= Time.deltaTime;
                }
            }
            else
            {
                rb.velocity = transform.forward * 5;
                transform.LookAt(boss.transform);
                minionAnimator.SetBool(IsRunning, true);
            }

            return;
        }
        
        
        minionAnimator.SetBool(IsRunning, GameManager.Instance.playerController.GetController().velocity.magnitude > 0);
        var rotDir = new Vector3(0, GameManager.Instance.playerController.joystick.Horizontal, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotDir * minionRotationAngle), rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (isStacked)
        {
            rb.velocity = Vector3.forward * 5;
        }
    }
    
    private void StartFighting()
    {
        rb.isKinematic = true;
        isAttacking = true;
        //minionAnimator.SetBool(IsRunning, false);
        minionAnimator.SetBool(IsFighting,  true);

    }

    public void FightBoss(BossController _boss)
    {
        boss = _boss;
        isFighting = true;

    }

    public void Cheer()
    {
        minionAnimator.SetTrigger(CheerID);
    }

    public void GetReadyToStack()
    {
        rb.isKinematic = false;
        
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionY;

        rb.useGravity = false;
        isStacked = true;
    }
    

    public void FinishStair()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        minionAnimator.SetBool(IsRunning, false);

        isFinishedStair = true;
    }
    public Rigidbody GetRigidBody()
    {
        return rb;
    }

    public Animator GetAnimator()
    {
        return minionAnimator;
    }

    public void KillMinion()
    {
        AudioManager.Instance.PlaySound2D("Death");
        var controller = GameManager.Instance.playerController;
        var deathParticle = LeanPool.Spawn(deathParticlePrefab, transform.position, Random.rotation);
       
        var decalPos = new Vector3(transform.position.x, transform.position.y - 0.9f, transform.position.z);
        var splatDecal = LeanPool.Spawn(minionSplatDecal, decalPos, Quaternion.Euler(90, 0, 0));
        
        var color = new Color(splatDecal.color.r, splatDecal.color.g, splatDecal.color.b, 1);
        splatDecal.color = color;
        
        splatDecal.DOFade(0, 5f).OnComplete(() =>
        {
            LeanPool.Despawn(splatDecal.gameObject);
        });
        
        LeanPool.Despawn(deathParticle, 1f);
        LeanPool.Despawn(gameObject,0.03f);
        
        controller.RemoveMinionsFromList(this);

    }

    public bool IsFinishedStair()
    {
        return isFinishedStair;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            StartFighting();
        }
    }
}
