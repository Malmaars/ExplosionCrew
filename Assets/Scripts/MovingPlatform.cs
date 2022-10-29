using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Transform platformToMove;
    Transform[] Locations;
    int currentIndex;
    public float moveSpeed;

    private void Awake()
    {
        platformToMove = transform.GetChild(0);

        Locations = new Transform[transform.childCount - 1];

        for(int i = 1; i < transform.childCount; i++)
        {
            Locations[i - 1] = transform.GetChild(i);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //platformToMove.position = Vector3.Lerp(platformToMove.position, Locations[currentIndex].position, moveSpeed * Time.deltaTime);

        Vector3 MoveDirection = Locations[currentIndex].position - platformToMove.position;
        platformToMove.position += MoveDirection.normalized * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(platformToMove.position, Locations[currentIndex].position) < 1f)
        {
            currentIndex++;
            if(currentIndex >= Locations.Length)
            {
                currentIndex = 0;
            }
        }
    }
}
