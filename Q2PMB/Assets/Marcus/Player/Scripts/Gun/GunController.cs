using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public InventoryManager inventory;

    public Transform collision;

    public Grenade Grenade;

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

            GrenadeController();
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


    void GrenadeController()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Grenade gren = Instantiate(Grenade.transform, inventory.weaponHolder).GetComponent<Grenade>();

            StartCoroutine(weightWait(.3f, 0));
            inventory.LArm.data.target = gren.LHand;
            gren.anim.CrossFade("Throw", .1f);



            inventory.rig.Build();
            StartCoroutine(ThrowGrenade(gren));

        }
    }

    IEnumerator weightWait(float time, float targetWeight)
    {
        if (time > Mathf.Epsilon)
        {
            for (float progress = 0; progress < time; progress += Time.deltaTime)
            {
                inventory.LArm.weight = Mathf.Lerp(inventory.LArm.weight, targetWeight, progress / time);
                yield return null;

            }
        }


    }
    IEnumerator ThrowGrenade(Grenade gren)
    {
        yield return new WaitForSeconds(.183f);
        StartCoroutine(weightWait(.2f, 1));

        yield return new WaitForSeconds(.283f);
        gren.anim.enabled = false;



        gren.transform.parent = null;
        gren.gameObject.AddComponent<Rigidbody>();
        gren.transform.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gren.transform.GetComponent<Rigidbody>().AddForce((Camera.main.transform.forward * 2 + Camera.main.transform.up / 1.3f) * 250);


        inventory.LArm.weight = 0;
        yield return new WaitForSeconds(.283f);
        inventory.LArm.data.target = inventory.currentWeapon.LHand;
        inventory.rig.Build();

        StartCoroutine(weightWait(.5f, 1));


    }
}
