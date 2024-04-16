using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerInfo : INetworkSerializable, IEquatable<PlayerInfo>
{
    public ulong _clientId;
    public FixedString32Bytes _Name;
    public bool _isPlayerReady;
    public Color _ColorID;

    //Serializable requirement
//structs must initilize values
    public PlayerInfo(ulong id)
    {
        _clientId = id;
        _ColorID = Color.magenta;
        _Name = "";
        _isPlayerReady = false;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _clientId);
        serializer.SerializeValue(ref _Name);
        serializer.SerializeValue(ref _isPlayerReady);
        serializer.SerializeValue(ref _ColorID);
    }

    //required for IEquatable
    public bool Equals(PlayerInfo other)
    {
        return _clientId == other._clientId;
    }
    
    
    //  help with fixdd strings
    public override string ToString() => _Name.Value.ToString();

    public static implicit operator string(PlayerInfo name) => name.ToString();
    public static implicit operator PlayerInfo(string s) => new PlayerInfo { _Name = new FixedString32Bytes(s) };

}
