using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : NetworkBehaviour
{
    //prefab we want to put into the content game obeject
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private Button enterBttn;
    [SerializeField] private TMP_Text inputText;
    [SerializeField] private GameObject ScrollViewContentGO;


    private ulong[] dmClientsIds = new ulong[2];
    private void Start()
    {
        //Subscribe to the on Click event from button
        enterBttn.onClick.AddListener(EnterEventBttn);
    }

    private void EnterEventBttn()
    {
       // NewMessage("player2", "hello messsage");
       
       SendMessageServerRpc(inputText.text, default);

    }


    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(string message, ServerRpcParams serverRpcParams)
    {
        // Cursing filter or ...
        
        //TODO add direct message logic here
        
        // @ClientID Message
        if(message.StartsWith("@"))
        {
            string[] parts = message.Split(" ");
            
            //get the client id from the first part of the split and remove the @ in the string
            string clientIdStr = parts[0].Replace("@", "");
            ulong toClientId = ulong.Parse(clientIdStr);

            
            
            
            ServerSendDirectMessage(message, serverRpcParams.Receive.SenderClientId, toClientId);

        }
        else
        {
            SendMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
        }
        
      
    }
    
    [ClientRpc]
    private void SendMessageClientRpc(string message, ulong clientID)
    {
        NewMessage(message, clientID.ToString());
    }
    
    [ClientRpc]
    private void RecieveMessageClientRpc(string message, ulong clientID, ClientRpcParams rpcParams)
    {
        NewMessage(message, clientID.ToString());
    }


    private void ServerSendDirectMessage(string message, ulong from, ulong to)
    {
        Debug.Log("from"+from+"to "+to);
        dmClientsIds[0] = from;
        dmClientsIds[1] = to;
        
        //create new rpc params with new target clients.
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = dmClientsIds;
       
        
        RecieveMessageClientRpc("whisper "+message.ToString(), from, rpcParams);


    }
    
    
    private void NewMessage(string message, string from)
    {
        if (message != "")
        {
            GameObject myMessage = Instantiate(MessagePrefab, ScrollViewContentGO.transform);


            myMessage.GetComponent<ChatMessageObj>().SetChatMessage(from, message);

            //reset the field
            inputText.text = "";

            //myMessage.transform.parent = ScrollViewContentGO.transform;
            // myMessage
        }
    }

}
