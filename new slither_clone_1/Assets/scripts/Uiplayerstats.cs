using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Uiplayerstats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lengthText;

    private void OnEnable()
    {
        Playerlength.ChangedLenghtEvent += ChangeLengthText;
    }

    private void OnDisable()
    {
        Playerlength.ChangedLenghtEvent -= ChangeLengthText;
    }

    private void ChangeLengthText(ushort lenght)
    {
        lengthText.text = lenght.ToString();
    }
}
