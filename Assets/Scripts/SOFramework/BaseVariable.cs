using System;
using UnityEngine;

namespace CyberneticStudios.SOFramework
{

    [Serializable]
    public abstract class BaseVariable : ScriptableObject
    {
        public abstract void ResetVariable();
    }
}
