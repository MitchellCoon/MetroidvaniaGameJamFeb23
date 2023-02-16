using System;
using UnityEngine;

namespace CyberneticStudios.SOFramework
{

    [CreateAssetMenu(menuName = "Conditions/Bool Condition")]
    public class BoolCondition : ScriptableObject
    {
        public enum ConditionType
        {
            IS_TRUE,
            IS_FALSE
        }

        [SerializeField] BoolVariable boolValue;
        [SerializeField] ConditionType conditionType;

        public bool value => GetValue();

        bool GetValue()
        {
            if (conditionType == ConditionType.IS_TRUE) return boolValue.value;
            return !boolValue.value;
        }
    }
}
