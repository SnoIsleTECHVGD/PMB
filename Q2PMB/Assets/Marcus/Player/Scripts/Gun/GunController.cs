using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public InventoryManager inventory;

    public Transform collision;

    public Transform cameraHolder;

    public Recoil recoil;
    public Grenade Grenade;

    private bool isAiming = false;
    private float shootTimer = 0;
    void Start()
    {

    }

    void Update()
    {
        if (inventory.currentWeapon)
        {
            shootTimer += Time.deltaTime;

            if (inventory.currentWeapon.GetComponent<GunObject>())
            {
                GunObject gun = inventory.currentWeapon.GetComponent<GunObject>();
                CollisionDetection(gun);
                HandleFiring(gun);
                HandleAnimation(gun);
            }

            GrenadeController();
        }
    }

    private void LateUpdate()
    {

    }


    private void HandleFiring(GunObject gun)
    {
        if (gun.gunInfo.isAutomatic)
        {
            if (Input.GetMouseButton(0) && shootTimer > gun.gunInfo.fireRate)
            {
                shootRecoil(gun);
                shootTimer = 0;

                Transform bullet = Instantiate(gun.gunInfo.bullet.transform);
                bullet.position = gun.Barrel.position;
                bullet.forward = Camera.main.transform.forward;
                bullet.GetComponent<Rigidbody>().AddForce(bullet.forward * 500, ForceMode.Impulse);
                Destroy(bullet.gameObject, 2);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && shootTimer > gun.gunInfo.fireRate)
            {
                shootRecoil(gun);
                shootTimer = 0;

                Transform bullet = Instantiate(gun.gunInfo.bullet.transform);
                bullet.position = gun.Barrel.position;
                bullet.forward = Camera.main.transform.forward;
                bullet.GetComponent<Rigidbody>().AddForce(bullet.forward * 500, ForceMode.Impulse);
                Destroy(bullet.gameObject, 2);
            }

        }
    }

    private void HandleAnimation(GunObject gun)
    {
        gun.anim.SetFloat("x", Input.GetAxisRaw("Horizontal"), .2f, Time.deltaTime);
        gun.anim.SetFloat("y", Input.GetAxisRaw("Vertical"), .2f, Time.deltaTime);

        if(Input.GetKey(KeyCode.LeftShift))
        {
            gun.anim.SetFloat("speed", 2);

        }
        else
        {
            gun.anim.SetFloat("speed", 1);
        }
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            gun.anim.SetBool("aiming", true);
        }
        else
        {
            isAiming = false;
            gun.anim.SetBool("aiming", false);
        }
    }

    private void shootRecoil(GunObject gun)
    {
        if(isAiming)
        {
            gun.anim.CrossFade("ShootAim", .05f);
        }
        else
        {
            gun.anim.CrossFade("Shoot", .05f);
        }

        recoil.recoilX = gun.gunInfo.recoilX;
        recoil.recoilY = gun.gunInfo.recoilY;
        recoil.recoilZ = gun.gunInfo.recoilZ;

        recoil.aimRecoilX = gun.gunInfo.aimRecoilX;
        recoil.aimRecoilY = gun.gunInfo.aimRecoilY;
        recoil.aimRecoilZ = gun.gunInfo.aimRecoilZ;

        recoil.snappiness = gun.gunInfo.snappiness;
        recoil.returnSpeed = gun.gunInfo.returnSpeed;

        recoil.Fire();

    }
    private void CollisionDetection(GunObject weapon)
    {
        RaycastHit hit;

        if (Physics.Raycast(weapon.RaycastPoint.position, cameraHolder.forward, out hit, weapon.gunInfo.raycastLength, ~inventory.ignore))
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
        if (Input.GetKeyDown(KeyCode.V))
        {
            Grenade gren = Instantiate(Grenade.transform, inventory.weaponHolder).GetComponent<Grenade>();

            StartCoroutine(weightWait(.3f, 0));
            gren.anim.CrossFade("Throw", .1f);

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
        yield return new WaitForSeconds(.283f);
        inventory.LArm.data.target = gren.LHand;
        inventory.rig.Build();
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
