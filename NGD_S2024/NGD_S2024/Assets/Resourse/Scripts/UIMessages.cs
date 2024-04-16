using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIMessages : MonoBehaviour
{
    [SerializeField]
    private GameObject myEntryPrefab;
    

    private void Start()
    {


        GameObject newEntry = Instantiate(myEntryPrefab);

        newEntry.GetComponent<MessageEntry>().NewTextMessage("player 2", "something");
        newEntry.transform.parent = gameObject.transform;

    }

    
}
