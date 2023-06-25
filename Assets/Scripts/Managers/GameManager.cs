using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("References")] 
    public PlayerController playerController;

    [Header("Levels")] 
    public LevelDataSO[] levels;
    private int currentLevel = 0;
    
    [Header("Ending")] 
    public float startingPointValue;
    public float pointValueToIncrement;
    public float currentBonusFactor;
    public bool isFinished;
    [SerializeField] private GameObject confettiParticle;
    private FinishManager finishLine;

    [Header("Money")] 
    public int moneyAmount;
    
    [Header("Scene Fade")] 
    [SerializeField] private Animator sceneFadeAnimator;
    [SerializeField] private float sceneFadeDuration;
    private static readonly int Fade = Animator.StringToHash("Fade");

    [Header("Upgrades")]
    public int startingMinionAmount;
    public int startingMinionCost = 100;
    [Space] 
    public float incomeFactor = 1;
    public int incomeFactorCost;
    private int currentIncomeFactorLevel = 1;
    private void Awake()
    {
        Instance = this;
        GameAnalytics.Initialize();
    }

    private void Start()
    {
        SetPrefs();
    }

    private void Update()
    {
        UIManager.Instance.LevelProgressBar(finishLine.transform,playerController.transform);
    }

    private void SetPrefs()
    {
        startingMinionAmount = PlayerPrefs.GetInt(PlayerPrefManager.CURRENT_MINION_STARTING_LEVEL, 1);
        startingMinionCost = PlayerPrefs.GetInt(PlayerPrefManager.CURRENT_MINION_STARTING_COST, 100);    
        
        incomeFactor = PlayerPrefs.GetFloat(PlayerPrefManager.CURRENT_INCOME_FACTOR, 1f);
        incomeFactorCost = PlayerPrefs.GetInt(PlayerPrefManager.INCOME_FACTOR_COST, 100);
        currentIncomeFactorLevel = PlayerPrefs.GetInt(PlayerPrefManager.CURRENT_INCOME_FACTOR_LEVEL, 1);
        
        moneyAmount = PlayerPrefs.GetInt(PlayerPrefManager.MONEY_AMOUNT, 100);
        
        UIManager.Instance.SetBuyMinionButtonUI(startingMinionAmount, startingMinionCost);
        UIManager.Instance.CheckMinionBuyButton(startingMinionCost, moneyAmount);
        
        UIManager.Instance.SetBuyIncomeFactorButtonUI(currentIncomeFactorLevel, incomeFactorCost);
        UIManager.Instance.CheckIncomeFactorBuyButton(incomeFactorCost, moneyAmount);
        
        UIManager.Instance.SetMoneyText(moneyAmount);

    }

    public void FailLevel()
    {
        UIManager.Instance.ShowEnding(false,0);
        isFinished = true;
    }

    public void WinLevel()
    {
        isFinished = true;
    }

    public void ShowWinScreen()
    {
        var rewardAmount = Mathf.RoundToInt(levels[currentLevel].rewardAmount * currentBonusFactor * incomeFactor);
        
        UIManager.Instance.ShowEnding(true,rewardAmount);
        IncreaseMoney(rewardAmount);
    }
    public void LoadScene(string sceneToLoad)
    {
        StartCoroutine(LoadSceneCO(sceneToLoad));
    }

    private IEnumerator LoadSceneCO(string sceneToLoad)
    {
        sceneFadeAnimator.SetTrigger(Fade);
        yield return new WaitForSeconds(sceneFadeDuration);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetFinishLine(FinishManager _finishLine)
    {
        finishLine = _finishLine;
    }

    public void PlayConfetti(Vector3 pos, Vector3 rot)
    {
        var confetti = LeanPool.Spawn(confettiParticle, pos, Quaternion.Euler(rot));
        LeanPool.Despawn(confetti, 5f);
    }

    public void IncreaseMoney(int amountToIncrease)
    {
        moneyAmount += amountToIncrease;
        
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Money",amountToIncrease, "Upgrade", "MoneyID");
        UIManager.Instance.SetMoneyText(moneyAmount);
        PlayerPrefs.SetInt(PlayerPrefManager.MONEY_AMOUNT, moneyAmount);
        PlayerPrefs.Save();
    }

    public void SpendMoney(int amount)
    {
        moneyAmount -= amount;
        if (moneyAmount <= 0)
        {
            moneyAmount = 0;
        }
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Money",amount, "Upgrade", GameAnalyticsDataManager.MONEY_ID);

        UIManager.Instance.SetMoneyText(moneyAmount);
        PlayerPrefs.SetInt(PlayerPrefManager.MONEY_AMOUNT, moneyAmount);
        PlayerPrefs.Save();
    }
    public void UpgradeStartingMinionLevel()
    {
        SpendMoney(startingMinionCost);
        
        startingMinionAmount++;
        startingMinionCost += startingMinionCost / 10;
        
        UIManager.Instance.SetBuyMinionButtonUI(startingMinionAmount, startingMinionCost);
        UIManager.Instance.CheckMinionBuyButton(startingMinionCost, moneyAmount);
        
        UIManager.Instance.CheckIncomeFactorBuyButton(incomeFactorCost, moneyAmount);

        playerController.SpawnMinion();
        
        PlayerPrefs.SetInt(PlayerPrefManager.CURRENT_MINION_STARTING_LEVEL, startingMinionAmount);
        PlayerPrefs.SetInt(PlayerPrefManager.CURRENT_MINION_STARTING_COST,startingMinionCost);
        PlayerPrefs.Save();
    }

    public void UpgradeIncomeFactor()
    {
        SpendMoney(incomeFactorCost);
        
        incomeFactor += incomeFactor * 0.2f;
        incomeFactorCost += incomeFactorCost / 10;
        currentIncomeFactorLevel++;
        
        UIManager.Instance.SetBuyIncomeFactorButtonUI(currentIncomeFactorLevel, incomeFactorCost);
        UIManager.Instance.CheckIncomeFactorBuyButton(incomeFactorCost, moneyAmount);
        
        UIManager.Instance.CheckMinionBuyButton(startingMinionCost, moneyAmount);
        
        PlayerPrefs.SetFloat(PlayerPrefManager.CURRENT_INCOME_FACTOR, incomeFactor);
        PlayerPrefs.SetInt(PlayerPrefManager.INCOME_FACTOR_COST, incomeFactorCost);
        PlayerPrefs.SetInt(PlayerPrefManager.CURRENT_INCOME_FACTOR_LEVEL, currentIncomeFactorLevel);
        PlayerPrefs.Save();
    }

    public LevelDataSO GetCurrentLevelData()
    {
        return levels[currentLevel];
    }
}
