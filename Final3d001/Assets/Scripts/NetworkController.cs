using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using TMPro;
using UnityEngine.UI;
//using ParrelSync;

public class NetworkController : MonoBehaviour
{
    [SerializeField] bool isHost;
    [SerializeField] TMP_InputField relayServerJoinCode;

    void Start()
    {
        //Init();
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        // Wait for UnityServices to initialize
        yield return UnityServices.InitializeAsync();

        // Sign in anonymously
        yield return AuthenticationService.Instance.SignInAnonymouslyAsync();

        // ClonesManager.IsClone()
        if (isHost)
        {
            var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(20, relayServerJoinCode);

            // Allocate a relay server
            yield return new WaitUntil(() => serverRelayUtilityTask.IsCompleted);
            RelayServerData relayServerData = serverRelayUtilityTask.Result;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            print(relayServerJoinCode.text);
            while (relayServerJoinCode.text == "" || relayServerJoinCode.text.Count() < 6)
            {
                yield return null;
            }

            print($"JOIN CODE {relayServerJoinCode.text}");
            var clientRelayUtilityTask = JoinRelayServerFromJoinCode(relayServerJoinCode.text);

            while (!clientRelayUtilityTask.IsCompleted)
            {
                yield return null;
            }

            if (clientRelayUtilityTask.IsFaulted)
            {
                Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
                yield break;
            }

            RelayServerData relayServerData = clientRelayUtilityTask.Result;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
    }


    //async void Init()
    //{
    //    await AuthenticatingAPlayer();

    //    if (isHost)
    //    {
    //        var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(20);
    //        var relayServerData = serverRelayUtilityTask.Result;
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
    //        NetworkManager.Singleton.StartHost();
    //    }
    //    else
    //    {
    //        NetworkManager.Singleton.StartClient();
    //    }
    //}

    static async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, TMP_InputField code, string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            code.text = $"JOIN CODE: {createJoinCode}";
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        return new RelayServerData(allocation, "dtls");
    }

    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        return new RelayServerData(allocation, "dtls");
    }

    //async Task AuthenticatingAPlayer()
    //{
    //    try
    //    {
    //        await UnityServices.InitializeAsync();
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //        Debug.Log("Sign in anonymously succeeded!");

    //        // Shows how to get the playerID
    //        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

    //    }
    //    catch (AuthenticationException ex)
    //    {
    //        // Compare error code to AuthenticationErrorCodes
    //        // Notify the player with the proper error message
    //        Debug.LogException(ex);
    //    }
    //    catch (RequestFailedException ex)
    //    {
    //        // Compare error code to CommonErrorCodes
    //        // Notify the player with the proper error message
    //        Debug.LogException(ex);
    //    }
    //}
}
