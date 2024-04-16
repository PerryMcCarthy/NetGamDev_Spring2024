using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{

    [SerializeField] private float _speed = 20f;

    [SerializeField] private float _DespawnTime = 6f;


    private void Update()
    {

        if (IsServer || IsHost)
        {
            if (_DespawnTime > 0.0f)
            {
                _DespawnTime -= Time.deltaTime;
            }
            else
            {

                ForceDespawnServerRpc();
            }
        }
    }

    [ServerRpc]
    private void ForceDespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
    //set velocity on spawn
    public override void OnNetworkSpawn()
    {
        GetComponent<Rigidbody>().velocity = this.transform.forward * _speed;
    }
    
    
    
}
