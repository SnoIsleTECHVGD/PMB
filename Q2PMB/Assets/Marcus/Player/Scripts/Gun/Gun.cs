using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Gun")]
public class Gun : ScriptableObject
{
    public bool isAutomatic;

    public float fireRate;

    public float recoilSnappiness;
    public float recoilReturnTime;

    public float recoilX;
    public float recoilY;
    public float recoilZ;

    public float aimRecoilX;
    public float aimRecoilY;
    public float aimRecoilZ;

    public float spreadFactor;

    public GameObject bullet;
}
