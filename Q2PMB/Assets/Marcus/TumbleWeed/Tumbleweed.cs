using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tumbleweed : MonoBehaviour
{
    public Vector3 windDirection;
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 currentVelocity = transform.position;

        transform.forward = windDirection;
        currentVelocity = transform.position + transform.forward * 3;

        currentVelocity = new Vector3(currentVelocity.x, currentVelocity.y - 1, currentVelocity.z);

        transform.position = Vector3.Lerp(transform.position, currentVelocity * Time.deltaTime, Time.deltaTime);
    }
}
