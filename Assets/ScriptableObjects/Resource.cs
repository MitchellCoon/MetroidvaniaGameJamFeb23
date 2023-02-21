using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "New Player Resource", menuName = "PlayerResource")]

public class Resource : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int maxValue;
    [SerializeField] int minValue;
    [SerializeField] int canvasChildNumber;
    [SerializeField] string staticText;

    private int currentValue;
    private Canvas canvas;

    public void Init()
    {
        currentValue = maxValue;
    }

    // private void OnEnable()
    // {
    //     currentValue = maxValue;
    //     canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    // }

    public int GetCurrentValue()
    {
        return currentValue;
    }

    public void UpdateUIElement()
    {
        Debug.Log("current value: " + currentValue);
        //canvas.transform.GetChild(canvasChildNumber).GetComponent<TextMeshProUGUI>().text = staticText + currentValue;
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
