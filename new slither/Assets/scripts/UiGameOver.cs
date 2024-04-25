using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGameOver : MonoBehaviour
{
    private Canvas _gameOverCanvas;

    private void Start()
    {
        _gameOverCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        Playercontrol.GameOverEvent += GameOver;
    }

    private void OnDisable()
    {
        Playercontrol.GameOverEvent -= GameOver;
    }

    private void GameOver()
    {
        _gameOverCanvas.enabled = true;
    }
    
}
