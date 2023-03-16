using UnityEngine;

public static class PlayerUtils
{
    public static PlayerMovementController FindPlayer(PlayerMovementController player)
    {
        if (IsValidPlayer(player))
        {
            return player;
        }
        GameObject playerObj = GameObject.FindWithTag(Constants.PLAYER_TAG);
        if (playerObj == null)
        {
            return null;
        }
        return playerObj.GetComponent<PlayerMovementController>();
    }

    private static bool IsValidPlayer(PlayerMovementController player)
    {
        if (player == null) return false;
        if (!player.gameObject.CompareTag(Constants.PLAYER_TAG)) return false;
        if (!player.isActiveAndEnabled) return false;
        return true;
    }
}
