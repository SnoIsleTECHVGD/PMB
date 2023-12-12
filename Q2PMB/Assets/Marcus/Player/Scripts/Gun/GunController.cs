using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public InventoryManager inventory;

    public Transform collision;

    void Start()
    {

    }

    void Update()
    {
        if (inventory.currentWeapon)
        {
            if (inventory.currentWeapon.GetComponent<GunObject>())
            {
                GunObject gun = inventory.currentWeapon.GetComponent<GunObject>();
                CollisionDetection(gun);

            }
        }
    }

    private void LateUpdate()
    {
        
    }

    private void CollisionDetection(GunObject weapon)
    {
        RaycastHit hit;

        if (Physics.Raycast(weapon.RaycastPoint.position, Camera.main.transform.forward, out hit, weapon.gunInfo.raycastLength, ~inventory.ignore))
        {
            Vector3 localHit = collision.InverseTransformPoint(hit.point);


            collision.localPosition = Vector3.Lerp(collision.localPosition, new Vector3(0, Mathf.Round(localHit.y * 100.0f) * 0.01f, 0), Time.deltaTime * 8);
        }
        else
        {
            collision.localPosition = Vector3.Lerp(collision.localPosition, Vector3.zero, Time.deltaTime * 8);
        }
    }
}
