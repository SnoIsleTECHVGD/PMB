using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class InventoryManager : MonoBehaviour
{
    public WeaponObject currentWeapon;
    public Transform weaponHolder;

    public RigBuilder rig;
    public TwoBoneIKConstraint LArm;
    public TwoBoneIKConstraint RArm;

    public LayerMask ignore;

    void Start()
    {
        if (currentWeapon)
        {
            EquipWeapon(currentWeapon);
        }
    }

    void Update()
    {
        if (currentWeapon)
        {
            DropHandler();
        }
        PickupHandler();

    }

    void DropHandler()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon(currentWeapon);
        }
    }

    void PickupHandler()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5, ~ignore))
            {
                print(hit.transform.name);
                if (hit.transform.GetComponent<WeaponObject>())
                {
                    EquipWeapon(hit.transform.GetComponent<WeaponObject>());
                }
            }
        }
    }
    public void EquipWeapon(WeaponObject weapon)
    {
        weapon.transform.parent = weaponHolder;
        Destroy(weapon.transform.GetComponent<Rigidbody>());
        weapon.transform.GetComponent<Collider>().enabled = false;
        weapon.anim.Rebind();
        weapon.anim.enabled = true;

        if (weapon.RHand)
        {
            RArm.data.target = weapon.RHand;
            RArm.weight = 1;
            if (weapon.RHint)
            {
                RArm.data.hint = weapon.RHint;
            }
        }

        if (weapon.LHand)
        {
            LArm.data.target = weapon.LHand;
            LArm.weight = 1;
            if (weapon.LHint)
            {
                LArm.data.hint = weapon.LHint;
            }
        }

        rig.Build();

        weapon.anim.enabled = true;

        weapon.anim.CrossFade("Equip", .1f);
        currentWeapon = weapon;
    }



    public void DropWeapon(WeaponObject weapon)
    {
        if (weapon.RHand)
        {
            RArm.data.target = null;
            RArm.weight = 0;
            if (weapon.RHint)
            {
                RArm.data.hint = null;
            }
        }

        if (weapon.LHand)
        {
            LArm.data.target = null;
            LArm.weight = 0;
            if (weapon.LHint)
            {
                LArm.data.hint = null;
            }
        }

        weapon.anim.enabled = false;

        weapon.transform.parent = null;
        weapon.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
        weapon.transform.GetComponent<Collider>().enabled = true;
        weapon.transform.gameObject.AddComponent<Rigidbody>();
        weapon.transform.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        weapon.transform.GetComponent<Rigidbody>().AddExplosionForce(25, transform.position, 10);
        currentWeapon = null;
    }
}
