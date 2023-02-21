using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    //Player Roataion
    float mouseX;
    public float lookSpeed;
    public Transform cameraTurn;
    float mouseY;
    float rotationX;

    //Player Movement
    float xAxis;
    float zAxis;
    public float moveSpeed;
    CharacterController cc;
    Vector3 v;

    //Gravity
    public bool isGrounded;
    float radius;
    public LayerMask groundLayerMask;
    public Transform groundCheck;
    float gravity;
    Vector3 velocity; // TODO: v & velocity is redundancy

    // TODO: Abstraction for Network class

    float oldRotationX = 0;
    float oldRotationY = 0;
    Vector3 moveVector = Vector3.zero;

    bool isLookingLocked = false;
    public static Vector3 spawnPosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Player Rotation
        lookSpeed = 250f;
        rotationX = 0f;

        //Player Movement
        moveSpeed = 40f;
        cc = GetComponent<CharacterController>();

        //Gravity
        isGrounded = false;
        radius = 0.6f;
        gravity = -9.81f * 2;

        spawnPosition = GameManager.GetAvailableSpawnPoint();
        transform.position = spawnPosition;
    }


    void Update()
    {
        if (IsClient && IsOwner)
        {
            UpdateFromClient();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TeleportToNextSpawnPoint();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {

            Cursor.visible = !Cursor.visible;
            if (!Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void TeleportToNextSpawnPoint()
    {
        spawnPosition = GameManager.GetAvailableSpawnPoint();
        transform.position = spawnPosition;
    }

    void UpdateFromClient()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isLookingLocked = !isLookingLocked;
        }

        if (!isLookingLocked)
        {
            UpdateRotation();
            transform.Rotate(0, mouseX, 0);
            cameraTurn.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        UpdateMovement();
        UpdateGravity();

        if (moveVector == Vector3.zero && velocity == Vector3.zero)
        {
            return;
        }

        cc.Move(new Vector3(moveVector.x, velocity.y, moveVector.z) * Time.deltaTime);
    }
    
    void UpdateRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
    }

    void UpdateMovement()
    {
        xAxis = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        zAxis = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if (zAxis == 0 && xAxis == 0)
        {
            moveVector = Vector3.zero;
            return;
        }

        v = transform.forward * zAxis + transform.right * xAxis;
        moveVector = v * moveSpeed;
    }

    void UpdateGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, radius, groundLayerMask);
        if (isGrounded == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += 10f;
        }
    }
}
