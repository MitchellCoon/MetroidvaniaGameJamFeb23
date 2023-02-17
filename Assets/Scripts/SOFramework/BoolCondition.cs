using System;
using UnityEngine;

namespace CyberneticStudios.SOFramework
{

    [Serializable]
    public class BoolCondition
    {
        [SerializeField] BoolVariable boolValue;
        [SerializeField] bool invert;

        public bool value => GetValue();

        public event System.Action<bool> OnChanged
        {
            // see: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/add
            add { boolValue.OnChanged += value; }
            remove { boolValue.OnChanged -= value; }
        }

        bool GetValue()
        {
            if (invert) return !boolValue.value;
            return boolValue.value;
        }
    }
}
