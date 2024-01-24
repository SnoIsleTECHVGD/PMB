using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePickup : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<GunController>())
        {
            other.transform.root.GetComponent<GunController>().grenadeCount++;
            other.transform.root.GetComponent<GunController>().pickupGrenade();

            Destroy(this.gameObject);
        }
    }
}
