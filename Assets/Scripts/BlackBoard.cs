using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlackBoard
{
    public static int collectedCollectibles;
    public static Transform CurrentCheckPoint;

    public static void TeleportObject(Vector3 location, Transform toMove)
    {
        toMove.position = location;
    }
}
