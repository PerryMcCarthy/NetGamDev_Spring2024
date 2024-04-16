using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayers : NetworkBehaviour
{
    public NetworkList<PlayerInfo> allNetPlayers;
    public int playerCount = 0;
    
    
    private Color[] playerColors = new Color[]
    {
        Color.blue,
        Color.magenta,
        Color.cyan,
        Color.yellow
    };
    //have to ini in awake, could cause memory leaks
    public void Awake()
    {
        allNetPlayers = new NetworkList<PlayerInfo>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (IsServer)
        {
            SeverStart();
        }
        Debug.Log("Player count ="+allNetPlayers.Count);
    }
    

    //seperated out starts
    private void SeverStart()
    {
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;

        int color = 0;
        foreach (NetworkClient nc in NetworkManager.ConnectedClientsList)
        {
            
            PlayerInfo info = new PlayerInfo(nc.ClientId);
            info._ColorID = playerColors[color];
            //match server client with the server make sure we are always ready
            if (nc.ClientId == NetworkManager.LocalClientId)
            {
                info._isPlayerReady = true;
            }
            allNetPlayers.Add(info);
            NetworkLog.LogInfo("color for client "+info._clientId+" is set to  "+color);
            color++;
            
           
        }

        playerCount = color;

    }

    //fires when a new client connects
    private void ServerOnClientConnected(ulong clientID)
    {
        PlayerInfo info = new PlayerInfo(clientID);
        info._isPlayerReady = false;
        allNetPlayers.Add(info);
        playerCount++;
        info._ColorID = playerColors[playerCount];
        
    }

    private int FindPlayerIndex(ulong clientID)
    {
        
        int myMatch = -1;
        //
        
        
        for(int i = 0; i > allNetPlayers.Count; i++)
        {
            if (clientID == allNetPlayers[i]._clientId)
            {
                myMatch = i;
            } 
            
        }
        
        /*
        foreach (NetworkClient nc in NetworkManager.ConnectedClientsList)
        {
            if (nc.ClientId == clientID)
            {
                // match found 
                myMatch = index;
            }else{}

            index++;
        }
        */
        return myMatch;
    }
    public void UpdateReadyClient(ulong clientID, bool isReady)
    {
        // lets get that index real quick
        int idx = FindPlayerIndex(clientID);
        if (idx == -1)
        {
            return;
        }

        // grab info, change it, and then send it back to the list
        PlayerInfo info = allNetPlayers[idx];
        info._isPlayerReady = isReady;
        allNetPlayers[idx] = info;
        
        
        NetworkLog.LogInfo("changing Ready !!  INDEX" + idx+" CLIENT "+clientID);

    }
}
