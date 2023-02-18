using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public GameObject mainCamera;

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            mainCamera.SetActive(true);
        }
    }

    private void Update()
    {
        if (mainCamera.activeSelf == false)
        {
            return;
        }

        //mainCamera.transform.position = transform.position;

        //mouseX = Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        //playerTransform.Rotate(0, mouseX, 0);

        //mouseY = Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        //cameraX -= mouseY;
        //cameraX = Mathf.Clamp(cameraX, -90, 90);

        //cameraTurn.Rotate(mouseY, 0, 0);
        //cameraTurn.localRotation = Quaternion.Euler(cameraX, 0, 0);
        //print($"CAMERA TURN: {cameraTurn}");
    }
}
