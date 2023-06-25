using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;
    
    [Header("Attacking")]
    [SerializeField] private int attackAnimations;
    [SerializeField] private float attackCooldown;
    private float lastAttack;
    private bool isFighting;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

    [Header("Health")] 
    [SerializeField] private UIBar healthbar;
    private int maxHealth;
    private int currentHealth;
    private bool isDead;
    private static readonly int Death = Animator.StringToHash("Death");


    private void Start()
    {
        //lastAttack = 4;
        healthbar.ToggleCanvasGroup(false);
    }

    private void Update()
    {
        if (!isFighting)
        {
            return;
        }

        if (lastAttack <= 0)
        {
            int attackIndex = Random.Range(0, attackAnimations);
            bossAnimator.SetTrigger(Attack);
            bossAnimator.SetInteger(AttackIndex, attackIndex);

            lastAttack = attackCooldown;
        }
        else
        {
            lastAttack -= Time.deltaTime;
        }
    }

    public void InitBoss(int bossHealth)
    {
        maxHealth = bossHealth;
        currentHealth = maxHealth;
        
        healthbar.SetMaxValue(maxHealth);
        healthbar.SetValue(currentHealth, maxHealth);
    }
    public void StartFighting()
    {
        isFighting = true;
        healthbar.ToggleCanvasGroup(true);
    }

    public void TakeDamage()
    {
        if (isDead)
        {
            return;
        }
        currentHealth--;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        healthbar.SetValue(currentHealth, maxHealth);
    }

    private void Die()
    {
        bossAnimator.SetBool(Death, true);
        GameManager.Instance.ShowWinScreen();
        foreach (var minionController in GameManager.Instance.playerController.MyMinions())
        {
            minionController.Cheer();
        }

        isDead = true;
    }
    
}
