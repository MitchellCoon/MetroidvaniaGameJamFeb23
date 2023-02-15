using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "New Player Resource", menuName = "PlayerResource")]

public class PlayerResource : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int maxValue;
    [SerializeField] int minValue;
    [SerializeField] int canvasChildNumber;
    [SerializeField] string staticText;

    private int currentValue;
    private Canvas canvas;

    private void OnEnable()
    {
        currentValue = maxValue;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void UpdateUIElement()
    {
        canvas.transform.GetChild(canvasChildNumber).GetComponent<TextMeshProUGUI>().text = staticText + currentValue;
    }

    public void AddResource(int amount)
    {
        currentValue = Mathf.Min(maxValue, currentValue + amount);
        UpdateUIElement();
    }

    public void SubtractResource(int amount)
    {
        currentValue = Mathf.Max(minValue, currentValue - amount);
        UpdateUIElement();
    }

}
