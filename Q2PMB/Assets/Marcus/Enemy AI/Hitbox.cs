using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damageMultiplier = 1;
    public void Damage(float minDamage, float maxDamage, Vector3 pos)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        transform.root.GetComponent<HealthController>().CurrentHealth -= Random.Range(minDamage, maxDamage) * damageMultiplier;
        transform.root.GetComponent<HealthController>().OnHit(pos);
        print("hit");
    }


}
