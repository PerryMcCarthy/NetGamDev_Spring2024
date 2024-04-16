using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TurretScript : NetworkBehaviour
{
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private Transform _firepoint; //or the starting postion of the bullet

    [SerializeField] private Button _FireBttn;
    // Start is called before the first frame update

    private GameObject _cursor;


    private void Awake()
    {
        _FireBttn.onClick.AddListener(FireProjectileClientRpc);
        _cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    private void Update()
    {
        
        if (IsHost || IsServer)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                FireProjectileClientRpc();
            }

            AimmingClientRpc();
        }
    }

    [ClientRpc]
    public void AimmingClientRpc()
    {
       
        _firepoint.LookAt(_cursor.transform, Vector3.forward);
    }
 
    

    [ClientRpc]
    public void FireProjectileClientRpc()
    {
            GameObject newProjectile = Instantiate(_projectile, _firepoint.position, _firepoint.rotation);
            newProjectile.GetComponent<NetworkObject>().Spawn();
    }
    

 
    
    
}
