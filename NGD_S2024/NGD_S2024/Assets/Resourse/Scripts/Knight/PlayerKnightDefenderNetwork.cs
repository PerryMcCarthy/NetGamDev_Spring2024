using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using Unity.Netcode;
using UnityEngine;

public class PlayerKnightDefenderNetwork : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool _serverAuth;

    private NetworkVariable<KnightNetworkState> _playerState = new NetworkVariable<KnightNetworkState>(writePerm:NetworkVariableWritePermission.Owner);

      
    private void Awake()
    {
        var permissions = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _playerState = new NetworkVariable<KnightNetworkState>(writePerm: permissions);
    }

    
    private void Update()
    {
        if (IsOwner)
        {
            TransmitPlayerState();
        }
    }

  

    #region Transmit State
    private void TransmitPlayerState()
    {
        var state = new KnightNetworkState()
        {
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles
        };

        if (IsServer || !_serverAuth)
        {
            _playerState.Value = state;
        }
        else
        {
            TransmitStateServerRPC(state);
        }
    }

    [ServerRpc]
    private void TransmitStateServerRPC(KnightNetworkState state)
    {
        _playerState.Value = state;
    }
    
    struct KnightNetworkState : INetworkSerializable
    {
        private Vector3 _pos;
        private short _yRot;
        
        internal Vector3 Position
        {
            get => _pos;
            set => _pos = value;
        }

        internal Vector3 Rotation
        {
            get => new Vector3(0, _yRot, 0);
            set => _yRot =  (short)value.y;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _pos);
            serializer.SerializeValue(ref _yRot);
        }
    }

    #endregion

//Ou


   
    public void setSpawn()
    {
        var spawnPointManager = GameObject.FindWithTag("Spawner").gameObject.GetComponent<KnightsSpwaner>();
        if (spawnPointManager != null)
        {
            // Set transform value here
            spawnPointManager.SetPlayerServerRpc(OwnerClientId);

        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
      // if(!IsOwner) Destroy(gameObject);
       if (IsOwner)
       {
          // setSpawn();
       }
       
    }
}
