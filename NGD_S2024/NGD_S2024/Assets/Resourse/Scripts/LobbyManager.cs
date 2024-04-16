using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FixedUpdate = UnityEngine.PlayerLoop.FixedUpdate;

public class LobbyManager : NetworkBehaviour
{
    //our UI buttons in the lobby UI
    [SerializeField] Button startBttn, leaveBttn, readyBttn;
    [SerializeField] private GameObject PanelPrefab;
    [SerializeField] private GameObject ContentGO;
    [SerializeField] private TMP_Text rdyTxt;
    
    //Public
    public NetworkPlayers _NetworkedPlayers;
   
    //Private
    private List<GameObject> _playerPanels = new List<GameObject>();
    private ulong myLocalClientID;
    private bool isReady = false;

    private void Start()
    {
        myLocalClientID = NetworkManager.LocalClientId;
        
        if (IsServer)
        {
            _NetworkedPlayers.allNetPlayers.OnListChanged += ServerOnNetPlayersChanged;
            ServerPopulateLabels();
            rdyTxt.text = "waiting for ready players";

        }
        else
        {
            ClientPopulateLabels();
            _NetworkedPlayers.allNetPlayers.OnListChanged += ClientOnAllPlayersChanged;
            
            readyBttn.onClick.AddListener(ClientRdyBttnToggled);
            rdyTxt.text = "not ready";
        }
        leaveBttn.onClick.AddListener(LeaveBttnClick);
        
      /*   old attempt
        if (IsHost)
        {
            foreach (NetworkClient nc in NetworkManager.ConnectedClientsList)
            {
               
                AddPlayerToList(nc.ClientId);
            }
            
            RefreshPlayerPanels();
        }
        
       
        */
    }

   

    [ServerRpc(RequireOwnership = false)]
    public void RdyBttnToggleServerRpc(bool readyStatus, ServerRpcParams serverRpcParams = default)
    {
        NetworkLog.LogInfo("RdyBttn serverRPC");
        _NetworkedPlayers.UpdateReadyClient(serverRpcParams.Receive.SenderClientId, readyStatus);
        ServerPopulateLabels();
        UpdatePlayerLabelsClientRpc();
    }

    [ClientRpc]
    public void UpdatePlayerLabelsClientRpc()
    {
        NetworkLog.LogInfo("RdyBttn clientRPC");
        if(!IsHost){ClientPopulateLabels();}
    }
    
    public void ClientRdyBttnToggled()
    {
        isReady = !isReady;
        if (isReady)
        {
            rdyTxt.text = "Ready!";
        }
        else
        {
            rdyTxt.text = "Not Ready!";
        }
        
        RdyBttnToggleServerRpc(isReady);
    }


    private void ServerPopulateLabels()
    {
        ClearPlayerPanels();
        foreach (PlayerInfo pi in _NetworkedPlayers.allNetPlayers)
        {

            GameObject newPanel = Instantiate(PanelPrefab, ContentGO.transform);
            LobbyPlayerLabel LPL = newPanel.GetComponent<LobbyPlayerLabel>();



          
                LPL.onKickClicked += KickUserBttn;
                // make sure we only the host or server displays kick button,
                if (pi._clientId == NetworkManager.LocalClientId)
                {
                    LPL.setKickActive(false);
                }
                else
                {
                    LPL.setKickActive(true);
                }

            
           //Display info and status status
            LPL.setPlayerName(pi._clientId);
            LPL.SetReady(pi._isPlayerReady);
            LPL.SetIconColor(pi._ColorID);
            _playerPanels.Add(newPanel);
        }
        
        //hides ready button
        readyBttn.GameObject().SetActive(false);
    }

    private void ClientPopulateLabels()
    {
        ClearPlayerPanels();
        foreach (PlayerInfo pi in _NetworkedPlayers.allNetPlayers)
        {

            GameObject newPanel = Instantiate(PanelPrefab, ContentGO.transform);
            LobbyPlayerLabel LPL = newPanel.GetComponent<LobbyPlayerLabel>();

            LPL.onKickClicked += KickUserBttn;
           //Turn off kick button for client.
            LPL.setKickActive(false);
            
            //Display info and status status
            LPL.setPlayerName(pi._clientId);
            LPL.SetReady(pi._isPlayerReady);
           
            LPL.SetIconColor(pi._ColorID);
            _playerPanels.Add(newPanel);
        }
        
        //show ready button and hide start button
       
    }
    
    
    private void ClearPlayerPanels()
    {
        foreach (GameObject panel in _playerPanels)
        {
            Destroy(panel);
        }

        _playerPanels.Clear();
    }

   


    //Bttns
    public void KickUserBttn(ulong kickTarget)
    {
        if (!IsServer || !IsHost) return;
        foreach (PlayerInfo pi in _NetworkedPlayers.allNetPlayers)
        {
            if (pi._clientId == kickTarget)
            {
                _NetworkedPlayers.allNetPlayers.Remove(pi);
                
                // send RPC to target client to discconnect/scene
                DisconnectClient(kickTarget);
               
            }
            }
      }
    
    private void LeaveBttnClick()
    {
        if (!IsServer)
        {
            QuitLobbyServerRpc();
        }
        else
        {
            NetworkManager.Shutdown();
        }
    }

    public void FixedUpdate()
    {
        if (NetworkManager.ShutdownInProgress)
        {
            SceneManager.LoadScene(0);
        }
    }


    //Server stuff
    private void ServerOnNetPlayersChanged(NetworkListEvent<PlayerInfo> changeevent)
    {
        ServerPopulateLabels();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void QuitLobbyServerRpc(ServerRpcParams serverRpcParams=default)
    {
        KickUserBttn(serverRpcParams.Receive.SenderClientId);
    }
    
    public void DisconnectClient(ulong kickTarget)
    {
        ClientRpcParams clientRpcParams = default;
        clientRpcParams.Send.TargetClientIds = new ulong[1] { kickTarget };
        DisconnectionClientRPC(clientRpcParams);
        NetworkManager.Singleton.DisconnectClient(kickTarget);
        
    }
    
 
//Client stuff

    private void ClientOnAllPlayersChanged(NetworkListEvent<PlayerInfo> changeEvent)
    {
        ClientPopulateLabels();
    }

    [ClientRpc]
    public void DisconnectionClientRPC(ClientRpcParams clientRpcParams)
    {
        SceneManager.LoadScene(0);
       
    }
    //Events
  
}
