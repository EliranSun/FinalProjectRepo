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
    // Server
    NetworkVariable<float> rotationXNetwork = new NetworkVariable<float>();
    NetworkVariable<float> rotationYNetwork = new NetworkVariable<float>();
    NetworkVariable<Quaternion> localRotationNetwork = new NetworkVariable<Quaternion>();
    NetworkVariable<Vector3> moveVectorNetwork = new NetworkVariable<Vector3>();
    NetworkVariable<Vector3> velocityNetwork = new NetworkVariable<Vector3>();

    float oldRotationX = 0;
    float oldRotationY = 0;
    Quaternion oldLocalRotation = Quaternion.identity;
    Vector3 oldMoveVector = Vector3.zero;
    Vector3 moveVector = Vector3.zero;
    Vector3 oldVelocity = Vector3.zero;

    bool isLookingLocked = false;
    Vector3 spawnPosition;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //Player Rotation
        lookSpeed = 500f;
        rotationX = 0f;

        //Player Movement
        moveSpeed = 40f;
        cc = GetComponent<CharacterController>();

        //Gravity
        isGrounded = false;
        radius = 0.6f;
        gravity = -9.81f;

        spawnPosition = GameManager.GetAvailableSpawnPoint();
        transform.position = spawnPosition;
        //transform.position = new Vector3(10, 0, 10);
    }

    void Update()
    {
        if (IsServer)
        {
            UpdateFromServer();
        }

        if (IsClient && IsOwner)
        {
            UpdateFromClient();
        }
    }

    void UpdateFromServer()
    {
        //transform.Rotate(0, localRotationNetwork.Value.y, 0);

        // TODO: I think the delay bug is because this is just an update to the server. We
        // need move in the client as well

        /**
         *      transform.Rotate(0, mouseX, 0);
                cameraTurn.localRotation = Quaternion.Euler(rotationX, 0, 0);
         */

        // TODO: Check if maybe the issue is that we need to add here local values,
        // rotationY + rotationYNetwork or moveVectorNetwork + moveVector
        transform.Rotate(0, rotationYNetwork.Value, 0);
        cameraTurn.localRotation = Quaternion.Euler(rotationXNetwork.Value, 0, 0);

        //cc.Move(moveVectorNetwork.Value);
        // cc.Move(velocityNetwork.Value + moveVectorNetwork.Value * Time.deltaTime);
        print($"Velocity: {velocityNetwork.Value} MoveVector: {moveVectorNetwork.Value} moveVector: {moveVector}");
        cc.Move(new Vector3(moveVectorNetwork.Value.x, velocityNetwork.Value.y, moveVectorNetwork.Value.z));
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
        }

        UpdateMovement();
        UpdateGravity();

        bool isMovementChanged = oldMoveVector != moveVector;
        bool isGravityChanged = oldVelocity != velocity;
        bool isRotationXChanged = oldRotationX != rotationX;
        bool isRotationYChanged = oldRotationY != mouseX;

        if (isMovementChanged || isGravityChanged || isRotationXChanged || isRotationYChanged)
        {
            oldMoveVector = moveVector;
            oldLocalRotation = transform.localRotation;
            oldVelocity = velocity;
            oldRotationX = rotationX;
            oldRotationY = mouseX;

            UpdateClientPositionServerRpc(
                moveVector,
                transform.localRotation,
                velocity,
                rotationX,
                mouseX
            );
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(
        Vector3 vector,
        Quaternion localRotation,
        Vector3 velocity,
        float rotationX,
        float rotationY
    )
    {
        moveVectorNetwork.Value = vector;
        localRotationNetwork.Value = localRotation;
        velocityNetwork.Value = velocity;
        rotationXNetwork.Value = rotationX;
        rotationYNetwork.Value = rotationY;
    }

    void UpdateRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        transform.Rotate(0, mouseX, 0);

        cameraTurn.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    void UpdateMovement()
    {
        // TODO: Put this back in a move method
        xAxis = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        zAxis = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        v = transform.forward * zAxis + transform.right * xAxis;
        moveVector = v * moveSpeed * Time.deltaTime;
    }

    void UpdateGravity()
    {
        if (Physics.CheckSphere(groundCheck.position, radius, groundLayerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            velocity.y += 6f;
        }
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
    }
}
