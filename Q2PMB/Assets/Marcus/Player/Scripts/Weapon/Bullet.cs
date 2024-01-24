 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float minDamage;
    public float maxDamage;
    public Transform defaultHit;
    public void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.GetComponent<Rigidbody>())
        {
            collision.transform.GetComponent<Rigidbody>().AddExplosionForce(600, collision.transform.position, 10);
            Destroy(gameObject);
        }
        else if(collision.transform.GetComponent<Hitbox>())
        {
            if(transform.name == "PlayerBullet" && collision.transform.name == "AIDetection")
            {

            }
            else
            {
                collision.transform.GetComponent<Hitbox>().Damage(minDamage, maxDamage, transform.position);
                Destroy(gameObject);
            }
            
        }
        else if(!collision.transform.root.GetComponent<Bullet>())
        {
            Transform hit = Instantiate(defaultHit);
            hit.position = transform.position;
            hit.localScale = new Vector3(.1f, .1f, .1f);
            Destroy(hit.gameObject, 2.3f);
            Destroy(gameObject);

        }


    }
}
