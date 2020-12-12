/*
    FILE:       PLayerScript.cs
    AUHTOR:     saphirako
    DATE:       20 OCT 2018

    DESCRIPTION:
    PlayerScript handles input from the user related to movement of the active Shape object.

    LEGAL:
    Copyright © 2020 Saphirako
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    private Rigidbody2D m_PlayerBody;            // Used to control the player's movements by changing its velocity
    private Vector2 m_Velocity;                  // Speed the player should move in 2D space
    private float m_PlayerSpeed = 10.0f;         // Player speed constant to be multiplied into Input 
    private float m_PlayerDefaultHeight = -4.0f;     // Y-coordinate to which the player tracks vertically
    private float mVerticalResistance = 0.1f;       // Constant multiplied to y-axis velocity to prevent incididental collisions between previous barriers and the new player
    private bool m_NewPlayer = true;                 // Represents whether the player has touched a barrier

    // Unity Methods:
    public void Awake () {
        m_PlayerBody = gameObject.GetComponent<Rigidbody2D> ();  
    }
    public void Update () {
        // Get input from keyboard:
        if (Input.GetAxis ("Horizontal") != 0) 
            m_Velocity.x = Input.GetAxis ("Horizontal") * m_PlayerSpeed;

        // Get input from touch:
        if (Input.touchCount > 0) {
            m_Velocity.x = (Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - transform.position.x) * m_PlayerSpeed;
            Debug.Log(m_Velocity);
        }

        // Correct player position after hitting the barrier / switching Player objects:
        m_Velocity.y = m_NewPlayer ? (m_PlayerDefaultHeight - transform.position.y) * mVerticalResistance * m_PlayerSpeed : 0;

        m_PlayerBody.velocity = m_Velocity;
    }

    public void OnCollisionEnter2D () {
        // Once the player object has touched the barrier, toggle the boolean for y-axis movement in m_Velocity. (It will now always be 0 to prevent the Rigidbody from bouncing.)
        m_NewPlayer = false;
    }

    public void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Despawner") {
            if (this.enabled) {
                GameManager.Instance.GameOver (true);
            }
        }
        // If we enter the keyhole:
        else if (other.tag == "Respawn") {
            //  Enable the new PlayerScript, update the GameManager to reflect the new player, and move change its transform to independent.
            PlayerScript newPlayer = other.transform.GetChild (0).GetComponent<PlayerScript> ();
            newPlayer.enabled = true;
            GameManager.Instance.SetNewPlayer (newPlayer);
            other.transform.DetachChildren ();

            // Move the now-defunct PlayerScript to be a child of the keyhole (for deletion) and disable the PlayerScript
            transform.SetParent (other.transform);
            this.enabled = false;
        }
    }
}