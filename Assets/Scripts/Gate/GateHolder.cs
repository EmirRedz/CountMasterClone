using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHolder : MonoBehaviour
{
    [SerializeField] private Gate[] gates;

    public void InitGates(List<GateData> datas)
    {
        for (var i = 0; i < gates.Length; i++)
        {
            var gate = gates[i];
            gate.InitGate(datas[i].gateType, datas[i].value);
        }
    }
}
