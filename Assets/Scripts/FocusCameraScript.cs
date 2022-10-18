using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FocusCameraScript
{
    public static void UpdatePositionBehindPlayer(Transform player, Transform target, Transform locBehindPlayer)
    {
        Vector3 playerMinusTarget = (player.position - target.position).normalized;
        playerMinusTarget = new Vector3(playerMinusTarget.x, 0, playerMinusTarget.z);
        locBehindPlayer.position = player.position + playerMinusTarget * 16 + Vector3.up * 3;
    }
}
