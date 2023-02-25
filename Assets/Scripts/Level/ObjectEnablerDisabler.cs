using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

public class ObjectEnablerDisabler : MonoBehaviour
{
    [SerializeField] BoolCondition conditionToEnable;

    public bool HasCondition => conditionToEnable != null && conditionToEnable.hasRef;
    public bool HasChildren => transform.childCount > 0;

    void Awake()
    {
        Assert.IsTrue(transform.childCount > 0, "Please add one or more children to this component to enable/disable");
    }
}
