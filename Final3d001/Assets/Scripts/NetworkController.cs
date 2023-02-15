using UnityEngine;
using Unity.Netcode;

public class NetworkController : MonoBehaviour
{
    [SerializeField] bool isHost = false;
    void Start()
    {
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        } else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}