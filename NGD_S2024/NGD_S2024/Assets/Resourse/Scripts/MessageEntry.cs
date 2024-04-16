using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageEntry : MonoBehaviour
{
   [SerializeField]
   private TMP_Text PlayerLbl, MessageLbl;



   public void NewTextMessage(string Player, string newMessage)
   {

      PlayerLbl.text = Player;
      MessageLbl.text = newMessage;
   }
   
}
