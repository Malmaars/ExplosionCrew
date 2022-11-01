using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = camera.position - transform.position;
        direction = new Vector3(direction.x, 0, direction.z);
        transform.forward = direction;
    }
}
