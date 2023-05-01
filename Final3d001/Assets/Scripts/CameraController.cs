using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public GameObject mainCamera;
    [SerializeField] private GameObject otherCamera;

    private void Start()
    {
        if (!IsClient)
        {
            return;
        };
        
        if (IsOwner)
        {
            mainCamera.SetActive(true);
            otherCamera.SetActive(false);
        }
        else
        {
            mainCamera.SetActive(false);
            otherCamera.SetActive(true);
        }
    }
}
