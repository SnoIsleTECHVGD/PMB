using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isCrouched;
    public bool isRunning;
    public bool canMove = true;
    public bool justJumped = false;

    [SerializeField] private LayerMask player;
    [SerializeField] private float walkSpeed = 2.4f;
    [SerializeField] private float runSpeed = 4;
    [SerializeField] private float jumpCooldown = 2;

    private CharacterController characterController;
    private float ySpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        bool isGrounded = characterController.isGrounded;

        jumpCooldown += Time.deltaTime;

        Vector3 move = Vector3.zero;
        float horizInput = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");

        ySpeed -= 9.81f * Time.deltaTime;

        if (canMove)
        {
            move = (transform.right * horizInput + transform.forward * vertInput).normalized;
        }

        var rayDown = new Ray(transform.position, Vector3.down * 2);

        RaycastHit hitDownInfo;
        Physics.Raycast(rayDown, out hitDownInfo, 3);

        if (hitDownInfo.normal.y < 1)
        {
            vertInput = hitDownInfo.normal.normalized.y;
        }
        else
        {
            vertInput = 0;
        }

        if (canMove)
        {
            if (Input.GetButtonDown("Jump") && justJumped == false && jumpCooldown > .9f)
            {
                jumpCooldown = 0;
                ySpeed += Mathf.Sqrt(1.2f * 1.3f * 9.81f);
                //just jumped
                justJumped = true;
            }
        }

        if (isGrounded && ySpeed < 0)
        {
            if (justJumped || ySpeed < -2)
            {

                var ray = new Ray(Camera.main.transform.position, Vector3.down);

              

                //just landed 
                justJumped = false;
            }
            ySpeed = 0f;
        }

        if (isGrounded)
            isRunning = Input.GetKey(KeyCode.LeftShift);

        move = calculateSpeed(move);
        move.y = ySpeed;

        characterController.Move(move * Time.deltaTime);

    }


    Vector3 calculateSpeed(Vector3 Move)
    {
        if (isCrouched)
        {
            return Move * 3;
        }
        if (isRunning && !isCrouched)
        {
            return Move * runSpeed;
        }
        return Move * walkSpeed;
    }
}
