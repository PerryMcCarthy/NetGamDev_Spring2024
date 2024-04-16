using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkinChanger : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject[] CharacterGO;

    public NetworkVariable<int> currentCharId = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentMatId = new NetworkVariable<int>(0);


    [SerializeField]
    private Material[] myMats;


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            setCharServerRpc((int)OwnerClientId);
            randomCharAndMatServerRpc();
        }
    }

    //Swap char
    [ServerRpc]
    public void setCharServerRpc(int charID)
    {
        foreach (GameObject cGO in CharacterGO)
        {
            cGO.SetActive(false);
        }
        currentCharId.Value = charID;
        CharacterGO[currentCharId.Value].SetActive(true);
    }
    
    [ServerRpc]
    public void setMatServerRpc(int matID)
    {
        // set materials
        currentMatId.Value = matID;
        CharacterGO[currentCharId.Value].GetComponent<SkinnedMeshRenderer>().material = myMats[matID];
    }
   
    [ServerRpc]
    public void randomCharAndMatServerRpc()
    {
        currentCharId.Value = Random.Range(0, CharacterGO.Length - 1);
        currentMatId.Value = Random.Range(0, myMats.Length - 1);
        setCharServerRpc(currentCharId.Value);
        setMatServerRpc(currentMatId.Value);


    }
   
    [ServerRpc]
    public void randomCharServerRpc()
    {
        currentCharId.Value = Random.Range(0, CharacterGO.Length - 1);
        setCharServerRpc(currentCharId.Value);
       
    }
   
    [ServerRpc]
    public void randomMatServerRpc()
    {
        currentMatId.Value = Random.Range(0, myMats.Length - 1);
        setMatServerRpc(currentMatId.Value);
       
    }
    
    
}
