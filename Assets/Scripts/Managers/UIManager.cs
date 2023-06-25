using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Ending")]
    [SerializeField] private GameObject failEnding;
    [SerializeField] private GameObject winEnding;
    [SerializeField] private GameObject rewardAmountGO;
    [SerializeField] private TMP_Text rewardAmountText;

    [Header("UI Progress Bar")] 
    [SerializeField] private Image progressFill;
    [SerializeField] private TMP_Text currentLevelText;
    
    [Header("Money")] 
    [SerializeField] private TMP_Text moneyText;
    
    [Header("Buy Minion Button")] 
    [SerializeField] private Button minionBuyButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;

    [Header("Buy Income Factor Button")] 
    [SerializeField] private Button incomeFactorBuyButton;
    [SerializeField] private TMP_Text incomeFactorLevelText;
    [SerializeField] private TMP_Text incomeFactorCostText;
    private void Awake()
    {
        Instance = this;
    }

    public void ShowEnding(bool isWin, int rewardAmount)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, GameAnalyticsDataManager.LEVEL_PROGRESS);

        if (isWin)
        {
            winEnding.SetActive(true);
        }
        else
        {
            failEnding.SetActive(true);
        }
        rewardAmountGO.SetActive(true);
        rewardAmountText.SetText("+" + rewardAmount);
    }

    public void LevelProgressBar(Transform finishLine, Transform player)
    {
        var playerZPosition = player.position.z;
        var distanceAmount = (playerZPosition - 0) / finishLine.transform.position.z;
        progressFill.fillAmount = distanceAmount;

    }

    public void SetMoneyText(int amount)
    {
        moneyText.SetText(amount.ToString());
    }

    public void SetBuyMinionButtonUI(int currentLevel, int currentCost)
    {
        levelText.SetText(currentLevel.ToString());
        costText.SetText(currentCost.ToString());
    }

    public void CheckMinionBuyButton(int currentCost, int amountToCheck)
    {
        if (currentCost > amountToCheck)
        {
            minionBuyButton.interactable = false;
        }
    }
    
    public void SetBuyIncomeFactorButtonUI(int currentLevel, int currentCost)
    {
        incomeFactorLevelText.SetText(currentLevel.ToString());
        incomeFactorCostText.SetText(currentCost.ToString());
    }
    
    public void CheckIncomeFactorBuyButton(int currentCost, int amountToCheck)
    {
        if (currentCost > amountToCheck)
        {
            incomeFactorBuyButton.interactable = false;
        }
    }

    public void SetCurrentLevelText(int level)
    {
        currentLevelText.SetText("Level " + level);
    }


}
