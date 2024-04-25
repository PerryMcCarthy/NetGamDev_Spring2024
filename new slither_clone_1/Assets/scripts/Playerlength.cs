using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Playerlength : NetworkBehaviour
{
    [SerializeField] private GameObject Tailprefab;
    
    public NetworkVariable<ushort> length = new(1, NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    [CanBeNull] public static event System.Action<ushort> ChangedLenghtEvent;

    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider2D;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();
        if (!IsServer) length.OnValueChanged += LengthChangedEvent;
        if (IsOwner) return;
        for (int i = 0; i < length.Value - 1; ++i)
            InstantiateTail();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        DestroyTails();
    }

    private void DestroyTails()
    {
        while (_tails.Count != 0)
        {
            GameObject tail = _tails[0];
            _tails.RemoveAt(0);
            Destroy(tail);
        }
    }
    
// This will be called by the server
    public void AddLength()
    {
        length.Value += 1;
        LengthChanged();

    }

    private void LengthChanged()
    {
        InstantiateTail();
        
        if(!IsOwner) return;
        ChangedLenghtEvent?.Invoke(length.Value);
        Clientmusicplayer.Instance.PlayNomAudioClip();
    }
    private void LengthChangedEvent(ushort previousValue, ushort newValue)
    {
        Debug.Log("LengthChange Callback");
        LengthChanged();
    }

    private void InstantiateTail()
    {
        GameObject tailGameObject = Instantiate(Tailprefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;
        if (tailGameObject.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameObject.transform;
            Physics2D.IgnoreCollision(tailGameObject.GetComponent<Collider2D>(), _collider2D);
        }
        _tails.Add(tailGameObject);
    }
}
