using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

public class ObjectEnablerDisabler : MonoBehaviour
{
    [SerializeField] BoolCondition conditionToEnable;

    public bool HasCondition => conditionToEnable != null && conditionToEnable.hasRef;
    public bool HasChildren => transform.childCount > 0;

    List<GameObject> children = new List<GameObject>();

    void OnEnable()
    {
        conditionToEnable.OnChanged += OnConditionChange;
    }

    void OnDisable()
    {
        conditionToEnable.OnChanged -= OnConditionChange;
    }

    void Awake()
    {
        Assert.IsTrue(HasCondition, "Please add a condition to ObjectEnablerDisabler");
        Assert.IsTrue(HasChildren, "Please add one or more children to this component to enable/disable");
        PopulateChildren();
    }

    void Start()
    {
        OnConditionChange(false);
    }

    void PopulateChildren()
    {
        children.Clear();
        foreach (Transform child in transform)
        {
            if (child != null) children.Add(child.gameObject);
        }
    }

    void OnConditionChange(bool incoming)
    {
        if (conditionToEnable.value)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    void Activate()
    {
        foreach (var child in children) if (child != null) child.SetActive(true);
    }

    void Deactivate()
    {
        foreach (var child in children) if (child != null) child.SetActive(false);
    }
}
