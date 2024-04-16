using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnHandler : NetworkBehaviour
{
  
    public override void OnNetworkSpawn()
    { base.OnNetworkSpawn();
        if (IsOwner)
        {
            setSpawn();
        }
        
    }


    public void setSpawn()
    {
        var spawnPointManager = GameObject.FindWithTag("Spawner").gameObject.GetComponent<KnightsSpwaner>();
        if (spawnPointManager != null)
        {
            // Set transform value here
         //   transform.position = spawnPointManager.GetNextSpawn().position;

        }
    }

}
