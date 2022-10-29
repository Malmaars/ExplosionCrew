using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleAnim : MonoBehaviour
{
    public float turnSpeed;
    public float upandDownSpeed;
    public float upAndDownHeight;

    int upOrDown = 1;

    float baseHeight;

    private void Awake()
    {
        baseHeight = transform.position.y;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + turnSpeed * Time.deltaTime, transform.rotation.eulerAngles.z));

        transform.position = new Vector3(transform.position.x, transform.position.y + upandDownSpeed * upOrDown * Time.deltaTime, transform.position.z);

        if(upOrDown == -1 && Mathf.Pow(Mathf.Pow(baseHeight - transform.position.y, 2), 0.5f) < 0.1f)
        {
            upOrDown = 1;
        }

        else if (upOrDown == 1 && Mathf.Pow(Mathf.Pow(baseHeight + upAndDownHeight - transform.position.y, 2), 0.5f) < 0.1f)
        {
            upOrDown = -1;
        }
    }
}
