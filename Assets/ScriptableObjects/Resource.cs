using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]

public class Resource : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int maxValue;
    [SerializeField] int minValue;
    [SerializeField] int canvasChildNumber;
    [SerializeField] string staticText;

    private int currentValue;
    private Canvas canvas;

    public event Action OnResourceUpdated;

    public void Init()
    {
        currentValue = maxValue;
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void SetCurrentValue(int newValue)
    {
        currentValue = newValue;
    }

    public float GetCurrentPercentage()
    {
        return (float)(Mathf.Max(currentValue - minValue, minValue)) / Mathf.Clamp(maxValue - minValue, minValue, maxValue);
    }

    public void AddResource(int amount)
    {
        currentValue = Mathf.Min(maxValue, currentValue + amount);
        OnResourceUpdated?.Invoke();
    }

    public void SubtractResource(int amount)
    {
        currentValue = Mathf.Max(minValue, currentValue - amount);
        OnResourceUpdated?.Invoke();
    }

    void OnValidate()
    {
        if (minValue < 0) minValue = 0;
        if (maxValue < minValue) maxValue = minValue;
    }
}
