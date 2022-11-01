using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontTouch : MonoBehaviour
{
    Material scrollingTexture;
    public float scrollingspeed;
    float currentOffset;

    public Transform respawnLocation;
    public bool lastLevel;

    private void FixedUpdate()
    {
        scrollingTexture = GetComponent<MeshRenderer>().material;
        currentOffset += scrollingspeed;
        scrollingTexture.SetTextureOffset(scrollingTexture.GetTexturePropertyNameIDs()[0], new Vector2(currentOffset, currentOffset));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //damage player;
            collision.transform.GetComponent<PlayerV2>().TakeDamage(1, collision.contacts[0].point);

            if (lastLevel)
            {
                BlackBoard.TeleportObject(BlackBoard.CurrentCheckPoint.position, collision.transform);
            }
            else
            {
                BlackBoard.TeleportObject(respawnLocation.position, collision.transform);
            }
        }
    }
}
