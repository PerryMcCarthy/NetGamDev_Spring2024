using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//usint input system
using UnityEngine.InputSystem;
//allows us to change mouse state
using UnityEngine.InputSystem.LowLevel;
// we can change inputusers
using UnityEngine.InputSystem.Users;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;


public class MouseInput : MonoBehaviour
{

    [SerializeField]
    private PlayerInput _playerInput;
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private RectTransform _cursorTransform;
    [SerializeField]
    private RectTransform _canvasRectTransform;
    [SerializeField] 
    private float _cursorSpeed = 1000f;

    [SerializeField] private float _padding = 50f;
    private bool _previousMouseState;
    
    private Mouse _virtualMouse;
    private Camera _mainCamera;

    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard";
    private string previousControlScheme = "";


    private void OnEnable()
    {
        _mainCamera = Camera.main;
        
        if (_virtualMouse == null)
        {
            // casting input into a mouse
            _virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!_virtualMouse.added)
        {
            InputSystem.AddDevice(_virtualMouse);
        }

        // link and pair the device to the user to use the  Player Input component with the event system in the virtual mouse.
        InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);

        if (_cursorTransform != null)
        {
            //get teh anchor point Transorm
            Vector2 position = _cursorTransform.anchoredPosition;
            
            //low level namepace call
            InputState.Change(_virtualMouse.position, position);
        }
        
        // update speed with our cursor transform
        //subscribe 
        InputSystem.onAfterUpdate += UpdateCursorMotion;
        _playerInput.onControlsChanged += OnControlsChange;
    }
    private void OnDisable()
    {
        InputSystem.RemoveDevice(_virtualMouse);
        InputSystem.onAfterUpdate -= UpdateCursorMotion;
        _playerInput.onControlsChanged -= OnControlsChange;
    }

    private void UpdateCursorMotion()
    {
        if (_virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        
        //delta, change between current and previous frame
        //  considering speed
        deltaValue *= _cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = _virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width - _padding); 
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height - -_padding);
        
        
        InputState.Change(_virtualMouse.position, newPosition);
        InputState.Change(_virtualMouse.delta, deltaValue);

        
        //Check to see if any button is pressed , could be any button at all
        bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
        if(_previousMouseState != aButtonIsPressed)
        {
            _virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, Gamepad.current.aButton.IsPressed());
            InputState.Change(_virtualMouse, mouseState);
            _previousMouseState = aButtonIsPressed;
        }
        
           
    
        // Set cursor to mouse

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, position, _canvas.renderMode
            == RenderMode.ScreenSpaceCamera ? null : _mainCamera, out anchoredPosition );
        
            //Fance way of doing an if statement in one line, says if the overlay is a screenspace, 
            //If its a screen space overlay? Yes? if its null, then pass in the main camera and output the positoin

            _cursorTransform.anchoredPosition = anchoredPosition;


    }

    //when player changes keryboard to mouse.
    private void OnControlsChange(PlayerInput input)
    {
        //switch to mouse
        if (_playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            _cursorTransform.gameObject.SetActive(false);
        
        
        }
    }
  
}
