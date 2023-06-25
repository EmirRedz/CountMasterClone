using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StairController : MonoBehaviour
{
    [SerializeField] private Renderer graphics;
    [SerializeField] private TMP_Text pointText;
    public float myPointValue;
    public void InitStairs(Material materialToSet, float pointValue)
    {
        graphics.sharedMaterial = materialToSet;
        myPointValue = pointValue;
        
        pointText.SetText("x" + myPointValue.ToString("F1"));
    }
}
