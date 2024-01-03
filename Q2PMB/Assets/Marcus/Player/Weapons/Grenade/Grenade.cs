using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Animator anim;
    public Transform LHand;
    public Transform LFingers;
    public Transform explosion;

    public float timeBeforeDetonate;
    public float explosionForce;
    public float explosionRadius;
    public float particleLifetime;
    public bool detonateOnCollision;

    private float timer;

    private bool exploded = false;
    private void Update()
    {
        if (!detonateOnCollision)
        {

            timer += Time.deltaTime;

            if (timeBeforeDetonate < timer)
            {

                Detonate();
                exploded = true;

            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (detonateOnCollision)
        {
            Detonate();
            exploded = true;
                
        }
    }

    private void Detonate()
    {
        if (exploded)
            return;

        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider coll in nearbyObjects)
        {
            if (coll.GetComponent<Rigidbody>())
            {

                coll.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);

            }
        }
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity).gameObject, particleLifetime);

        Destroy(this.gameObject, 1);
    }
}
