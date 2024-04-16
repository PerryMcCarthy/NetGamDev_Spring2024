using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMessageObj : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text chatMessage;


    public void SetChatMessage(string name, string message)
    {
        playerName.text = name;
        chatMessage.text = message;
    }
    
}
