using System.Collections;
using System.Collections.Generic;
using Resourse.Scripts.Tank;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class TankSpawner : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject PlayerPrefab;
    public Camera mainCamera;
    public bool isStarted = false;
    
    private int positionIndex = 0;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(4, .5f, 0),
        new Vector3(-4, .5f, 0),
        new Vector3(0, .5f, 4),
        new Vector3(0, .5f, -4)
    };

    private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
    };
   

    public override void OnNetworkSpawn()
    { 
       
       // mainCamera.GetComponent<AudioListener>().enabled = !IsClient;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    private void SceneLoaded(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if(isStarted)return;
        if (IsHost && scenename == "tanks")
        {
            foreach (ulong clientID in clientscompleted)
            {
                //spawn players

                // instatiate a new player
                GameObject newPlayer = Instantiate(PlayerPrefab, NextPosition(), quaternion.identity);
                newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
                newPlayer.GetComponent<TankPlayerManager>().m_PlayerColor.Value = NextColor();

            }
        }

        isStarted = true;
        


    }



    
    private Vector3 NextPosition() {
        Vector3 pos = startPositions[positionIndex];
        positionIndex += 1;
        if (positionIndex > startPositions.Length - 1) {
            positionIndex = 0;
        }
        return pos;
    }


    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1) {
            colorIndex = 0;
        }
        return newColor;
    }

}
