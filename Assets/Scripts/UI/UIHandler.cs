using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(1000)]
public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    // public void UpdateUIElement(string newText)
    // {
    //     healthText.SetText("Health: " + health);
    // }
}
