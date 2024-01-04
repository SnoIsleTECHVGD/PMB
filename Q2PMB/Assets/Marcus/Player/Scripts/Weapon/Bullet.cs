using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float minDamage;
    public float maxDamage;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<Rigidbody>())
        {
            collision.transform.GetComponent<Rigidbody>().AddExplosionForce(600, collision.transform.position, 10);
            Destroy(gameObject);
        }
    }
}
