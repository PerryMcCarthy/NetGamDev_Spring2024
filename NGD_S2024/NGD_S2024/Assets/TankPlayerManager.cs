using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Resourse.Scripts.Tank
{
public class TankPlayerManager : NetworkBehaviour
{
        // This class is to manage various settings on a tank.
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game.

        public NetworkVariable<Color> m_PlayerColor = new NetworkVariable<Color>(Color.blue);                             // This is the color this tank will be tinted.
        
                                 // The position and direction the tank will have when it spawns.
        [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
        [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.

        [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.
        
        [SerializeField]
        private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control.
        [SerializeField]
        private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control.
        [SerializeField]
        private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.
       
        [SerializeField]
        private Camera m_playerCamera;

        [SerializeField] private AudioListener m_AudioLister;

        private Quaternion m_StartingROT;
        private Vector3 m_StartingPOS;

        public override void OnNetworkSpawn(){
            
            
            m_playerCamera = gameObject.GetComponentInChildren<Camera>();
          
            m_AudioLister  = gameObject.GetComponentInChildren<AudioListener>();
            m_playerCamera.enabled = IsOwner;
            m_AudioLister.enabled = IsOwner;
            NetworkIni();
            base.OnNetworkSpawn();
           
        }

        public void NetworkIni ()
        {

            m_PlayerColor.OnValueChanged += OnPlayerColorChange;

            m_StartingPOS = transform.position;
            m_StartingROT = transform.rotation;
     
            
            ApplyPlayerColor();
        }

        private void ApplyPlayerColor()
        {
            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
          //  m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber +"</color>";

            // Get all of the renderers of the tank.
            MeshRenderer[] renderers = m_CanvasGameObject.GetComponentsInChildren<MeshRenderer>();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... set their material color to the color specific to this tank.
                renderers[i].material.color = m_PlayerColor.Value;
            }
        }

        private void OnPlayerColorChange(Color previousvalue, Color newvalue)
        {
            ApplyPlayerColor();
        }



        // Used during the phases of the game where the player shouldn't be able to control their tank.
        public void DisableControl ()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;

            m_CanvasGameObject.SetActive (false);
        }


        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl ()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_CanvasGameObject.SetActive (true);
        }


        // Used at the start of each round to put the tank into it's default state.
        public void Reset ()
        {
            transform.position = m_StartingPOS;
            transform.rotation = m_StartingROT;

            // m_CanvasGameObject.SetActive (false);
          //  m_CanvasGameObject.SetActive  (true);
        }
    }
}
