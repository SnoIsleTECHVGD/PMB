using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;
    public float regenRate;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    private void Update()
    {
        
    }

    public virtual void OnHit(Vector3 pos)
    {

    }

}
