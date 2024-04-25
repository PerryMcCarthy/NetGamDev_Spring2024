using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionAprovelhanler : MonoBehaviour
{
    private const int MaxPlayer = 10;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager
        .ConnectionApprovalResponse response)
    {
        Debug.Log("Connect Approval");
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayer)
        {
            response.Approved = false;
            response.Reason = "Server is FULL";
        }

        response.Pending = false;
    }
}
