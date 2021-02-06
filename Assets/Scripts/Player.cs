﻿/*
    FILE:       Playes
    AUHTOR:     saphirako
    DATE:       20 OCT 2018

    DESCRIPTION:
    Playeres input from the user related to movement of the active Shape object.

    LEGAL:
    Copyright © 2020 Saphirako
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Current;         // Represents the player object that the user is currently controlling
    public BarrierManager.Shape Type;       // Represents the shape of the player object


    // ///////////////////////////////
    // Player prefabs: These contain the GameObjects used to create player objects    
    // ///////////////////////////////
    private static GameObject circle_prefab;
    private static GameObject triangle_prefab;
    private static GameObject square_prefab;
    // ///////////////////////////////

    private Rigidbody2D rb;            // Used to control the player's movements by changing its velocity
    private Vector2 mVelocity;                  // Speed the player should move in 2D space
    private float playerSpeed = 10.0f;         // Player speed constant to be multiplied into Input 
    private float playerDefaultHeight = -4.0f;     // Y-coordinate to which the player tracks vertically
    private float verticalResistance = 0.1f;       // Constant multiplied to y-axis velocity to prevent incididental collisions between previous barriers and the new player
    private bool isNewPlayer = true;                 // Represents whether the player has not touched a barrier


    // Unity Methods:
    public void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Update() {
        // We only want user input to control the current player object
        if (this == Current) {
            // Get input from keyboard:
            if (Input.GetAxis("Horizontal") != 0)
                mVelocity.x = Input.GetAxis("Horizontal") * playerSpeed;

            // Get input from touch:
            if (Input.touchCount > 0) {
                mVelocity.x = (Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x - transform.position.x) * playerSpeed;
                Debug.Log(mVelocity);
            }

            // // Correct player position after hitting the barrier / switching Player objects:
            // mVelocity.y = isNewPlayer ? (playerDefaultHeight - transform.position.y) * verticalResistance * playerSpeed : 0;

            // rb.velocity = mVelocity;
        }
    }

    public static void InitializeResources() {
        circle_prefab = Resources.Load<GameObject>("Prefabs/Player (Circle)");
        square_prefab = Resources.Load<GameObject>("Prefabs/Player (Square)");
        triangle_prefab = Resources.Load<GameObject>("Prefabs/Player (Triangle)");

        if (!circle_prefab || !square_prefab || !triangle_prefab) {
            Debug.LogError("Failed to load player prefabs!");
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
        }
        Debug.Log("Player prefabs loaded succesfully");
    }

    public static Player CreateNewPlayer(BarrierManager.Shape requiredShape, bool isFirstPlayer = false) {
        Player newPlayer;

        switch (requiredShape) {
            case BarrierManager.Shape.CIRCLE:
                newPlayer = Instantiate(circle_prefab, GameManager.Instance.InitialSpawn).GetComponent<Player>();
                break;

            case BarrierManager.Shape.TRIANGLE:
                newPlayer = Instantiate(triangle_prefab, GameManager.Instance.InitialSpawn).GetComponent<Player>();
                break;

            case BarrierManager.Shape.SQUARE:
                newPlayer = Instantiate(square_prefab, GameManager.Instance.InitialSpawn).GetComponent<Player>();
                break;

            default:
                Debug.LogError("Error creating a new player. Defaulting to Shape.CIRCLE.");
                newPlayer = Instantiate(circle_prefab, GameManager.Instance.InitialSpawn).GetComponent<Player>();
                break;
        }

        // Set the Player.Type member to be the required shape
        newPlayer.Type = requiredShape;

        // If this is the first player of the game, we need to set it to the active player object
        if (isFirstPlayer)
            SetCurrentPlayer(newPlayer);
        
        return newPlayer;
    }

    public static void SetCurrentPlayer(Player p) {
        Current = p;
        p.gameObject.name = "Player";
    }

    // Move():          Moves the player object
    public void Move(bool isExternalForce = false) {
        // If this is movement call from a different source other than input (ie. Barrier spawn), attach the same force
        if (isExternalForce) rb.AddForce(Vector2.down * BarrierManager.Speed);
    }

    public void OnCollisionEnter2D() {
        // Once the player object has touched the barrier, toggle the boolean for y-axis movement in mVelocity. (It will now always be 0 to prevent the Rigidbody from bouncing.)
        isNewPlayer = false;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Despawner") {
            if (this.enabled) {
                GameManager.Instance.GameOver(true);
            }
        }
        // If we enter the keyhole:
        else if (other.tag == "Respawn") {
            //  Enable the new Player and update the GameManager to reflect the new player, and move change its transform to independent.
            Player newPlayer = other.transform.GetChild(0).GetComponent<Player>();
            newPlayer.enabled = true;
            SetCurrentPlayer(newPlayer);
            other.transform.DetachChildren();

            // Move the now-defunct Player a child of the keyhole (for deletion) and disable the Player
            transform.SetParent(other.transform);
            this.enabled = false;
        }
    }
}