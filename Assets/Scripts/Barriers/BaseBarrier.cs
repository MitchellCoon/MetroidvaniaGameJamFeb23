using UnityEngine;

using CyberneticStudios.SOFramework;

public abstract class BaseBarrier : MonoBehaviour
{
    [SerializeField] BoolCondition[] openConditions = new BoolCondition[0];

    // TODO: make this the base class for Gate and LockedDoor
}
