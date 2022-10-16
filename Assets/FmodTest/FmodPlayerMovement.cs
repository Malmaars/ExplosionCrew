using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodPlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            transform.position += new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.position -= new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
    }
}
