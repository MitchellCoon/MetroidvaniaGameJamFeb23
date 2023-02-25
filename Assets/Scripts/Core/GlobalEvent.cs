using System;
using UnityEngine;

// The `event` keyword actually removes functionality from the Action, making it so that
// we can only call `Invoke` from within the same class.

// This does add some extra boilerplate but gives us more safety of not accidentally
// overriding event subscriptions.

public static class GlobalEvent
{
    public static event Action<Vector2> OnRoomLoaded;
    public static event Action<GameObject> OnPlayerSpawn;
    public static event Action OnPlayerDeath;
    public static event Action OnEmergencyPlayerInstakillSomethingWentHorriblyWrong;

    public static class Invoke
    {
        public static void OnRoomLoaded(Vector2 roomPosition) { GlobalEvent.OnRoomLoaded?.Invoke(roomPosition); }
        public static void OnPlayerSpawn(GameObject playerObj) { GlobalEvent.OnPlayerSpawn?.Invoke(playerObj); }
        public static void OnPlayerDeath() { GlobalEvent.OnPlayerDeath?.Invoke(); }
        public static void OnEmergencyPlayerInstakillSomethingWentHorriblyWrong() { GlobalEvent.OnEmergencyPlayerInstakillSomethingWentHorriblyWrong?.Invoke(); }
    }
}
