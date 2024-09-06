using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;
public class SpendResearch : MonoBehaviour
{
    public int cost;
    public int researchTier;
    public int maxResearchTier;

    public UnityEvent onSpendResearch;
    void Start()
    {

    }
    void Update()
    {
    }
    public virtual void BuyNext()
    {
            onSpendResearch?.Invoke();
    }

    public void AddListener(UnityAction a)
    {
        onSpendResearch.AddListener(a);
    }
}
