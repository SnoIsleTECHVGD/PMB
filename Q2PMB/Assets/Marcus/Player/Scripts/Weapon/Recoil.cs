using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public bool isAiming = false;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public float aimRecoilX;
    public float aimRecoilY;
    public float aimRecoilZ;

    public float returnSpeed;
    public float snappiness;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Fire()
    {
        if (isAiming)
        {
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
    }
}
