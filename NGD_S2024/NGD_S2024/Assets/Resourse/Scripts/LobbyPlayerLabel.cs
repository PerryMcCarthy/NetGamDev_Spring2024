using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerLabel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text PlayerText;
    [SerializeField] private Image readyImage, colorImage;
    [SerializeField] private Button kickBttn;

    public event Action<ulong> onKickClicked;
    private ulong _clientID;

    private void OnEnable()
    {
        kickBttn.onClick.AddListener(BtnKick_Clicked);
     ;
    }

    public void setPlayerName(ulong playerName)
    {
        _clientID = playerName;
        PlayerText.text = "Player "+playerName.ToString();
    }
    
    private void BtnKick_Clicked()
    {
        onKickClicked?.Invoke(_clientID);
    }

    public void setKickActive(bool isOn)
    {
        kickBttn.gameObject.SetActive(isOn);
    }

    public void SetReady(bool ready)
    {
        if (ready)
        {
            readyImage.color = Color.green;
        }
        else
        {
            readyImage.color = Color.red;
        }
    }

    public void SetIconColor(Color color)
    {
        colorImage.color = color;
    }

    
    
}
