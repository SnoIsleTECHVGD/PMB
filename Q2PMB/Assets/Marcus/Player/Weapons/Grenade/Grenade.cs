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
        Collider[] nearbyObjects2 = Physics.OverlapSphere(transform.position, explosionRadius * 2.5f);

        foreach (Collider coll in nearbyObjects)
        {
            if (coll.GetComponent<Rigidbody>())
            {

                coll.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);

            }

            if(coll.GetComponent<Hitbox>())
            {
                coll.GetComponent<Hitbox>().Damage(125 / Vector3.Distance(transform.position, coll.transform.position), 125 / Vector3.Distance(transform.position, coll.transform.position), coll.transform.position);

                
            }
        }

        foreach (Collider coll in nearbyObjects2)
        {
            
            if (coll.GetComponent<Hitbox>())
            {
                if (coll.transform.root.GetComponent<Player>())
                {
                    Camera.main.GetComponent<CamShake>().trauma = .25f;

                    Camera.main.GetComponent<CamShake>().Shake(1);
                }

            }
        }

       
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity).gameObject, particleLifetime);

        Destroy(this.gameObject, .1f);
    }
}
