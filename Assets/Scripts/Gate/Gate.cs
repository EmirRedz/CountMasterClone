using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Values")]
    [SerializeField] private GateType type;
    [SerializeField] private int value;
    private bool isGateTriggered;
    [Header("Gate Visuals")] 
    [SerializeField] private GameObject goodParticle;
    [SerializeField] private GameObject badParticle;
    [SerializeField] private TMP_Text gateText;
    

    public void InitGate(GateType _type, int _value)
    {
        type = _type;
        value = _value;
        
        switch (type)
        {
            case GateType.Add:
                goodParticle.SetActive(true);
                badParticle.SetActive(false);
                gateText.SetText("+" + value);
                break;
            case GateType.Subtract:
                goodParticle.SetActive(false);
                badParticle.SetActive(true);
                gateText.SetText("-" + value);

                break;
            case GateType.Multiply:
                goodParticle.SetActive(true);
                badParticle.SetActive(false);
                gateText.SetText("X" + value);
                break;
        }
    }

    public void TriggerGate(PlayerController controller)
    {
        if (isGateTriggered)
        {
            return;
        }
        switch (type)
        {
            case GateType.Add:
                for (int i = 0; i < value; i++)
                {
                    controller.SpawnMinion();
                }
                break;
            case GateType.Subtract:
                int valueActual = value >= controller.MyMinions().Count ? controller.MyMinions().Count : value;
                // for (int i = 0; i < valueActual; i++)
                // {
                //     var minion = controller.MyMinions()[i];
                //     Debug.Log("Index: " + i);
                //     if (minion != null)
                //     {
                //         minion.KillMinion();
                //     }
                // }

                Debug.Log(valueActual);
                for (int i = valueActual - 1; i >= 0; i--)
                {
                    var minion = controller.MyMinions()[i];
                    Debug.Log("Index: " + i);
                    if (minion != null)
                    {
                        minion.KillMinion();
                    }
                }
                break;
            case GateType.Multiply:
                int amountToAdd = value * controller.CurrentMinionCount();
                amountToAdd -= controller.CurrentMinionCount();
                for (int i = 0; i < amountToAdd; i++)
                {
                    controller.SpawnMinion();
                }
                break;
        }

        //transform.DOPunchScale(Vector3.one * 0.75f, 0.25f);
        AudioManager.Instance.PlaySound2D("Gate");
        controller.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
        gameObject.SetActive(false);

        isGateTriggered = true;
        
    }
    
    public enum GateType
    {
        Add,
        Subtract,
        Multiply,
    }
}
