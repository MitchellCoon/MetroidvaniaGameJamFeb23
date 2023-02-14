using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Resource", menuName = "PlayerResource")]

public class PlayerResource : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int maxValue;
    [SerializeField] int minValue;
    private int currentValue;

    private void OnEnable()
    {
        currentValue = maxValue;
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void AddResource(int amount)
    {
        currentValue = Mathf.Min(maxValue, currentValue + amount);
    }

    public void SubtractResource(int amount)
    {
        currentValue = Mathf.Max(minValue, currentValue - amount);
    }

}
