using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimController : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement movement;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = transform.root.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        
        anim.SetFloat("x", Input.GetAxisRaw("Horizontal"), .1f, Time.deltaTime);
        anim.SetFloat("y", Input.GetAxisRaw("Vertical"), .1f, Time.deltaTime);

        if (movement.isRunning)
        {
            anim.SetFloat("speed", 2);
        }
        else
        {
            anim.SetFloat("speed", 1);
        }


    }
}
