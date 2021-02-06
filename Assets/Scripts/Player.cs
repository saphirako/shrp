/*
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
    private static Transform idleTrackingTarget;     // Position to which player automatically moves when there is no input (when idling)


    // ///////////////////////////////
    // Player prefabs: These contain the GameObjects used to create player objects    
    // ///////////////////////////////
    private static GameObject circle_prefab;
    private static GameObject triangle_prefab;
    private static GameObject square_prefab;
    // ///////////////////////////////

    private Rigidbody2D rb;            // Used to control the player's movements by changing its velocity
    private Vector2 playerMovementVector;                  // Speed the player should move in 2D space
    private float playerSpeed = 20f;         // Player speed constant to be multiplied into Input 
    private float idleInputDelay = .1f;      // Delay (in seconds) between last time there was input and tracking to the idle position
    private float lastInputTime;
    private bool isNewPlayer = true;                 // Represents whether the player has not touched a barrier


    // Unity Methods:
    public void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Update() {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.touchCount > 0) lastInputTime = idleInputDelay;
        else if (lastInputTime > 0) lastInputTime -= Time.deltaTime;
        Current.Move();
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

        idleTrackingTarget = GameObject.Find("Player Movement Target").transform;
        Debug.Log("Player Idle Target succesfully initialized");
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

        // This is direct input for the current player
        else {
            if (this == Current) {
                rb.velocity = Vector2.zero;
                Vector2 movement;
                
                // Get input from keyboard:
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                    playerMovementVector.x = Input.GetAxis("Horizontal");
                    playerMovementVector.y = Input.GetAxis("Vertical");
                    movement = playerMovementVector * playerSpeed;
                }

                // Get input from touch:
                else if (Input.touchCount > 0) {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    playerMovementVector.x = touchPosition.x - transform.position.x;
                    playerMovementVector.y = touchPosition.y - transform.position.y;
                    movement = playerMovementVector * playerSpeed;
                }

                // There is no input and we're beyond the delay time for idling, auto track to center position
                else {
                    movement = Vector2.zero;
                    if (lastInputTime <= 0) {
                        playerMovementVector.x = idleTrackingTarget.position.x - transform.position.x;
                        playerMovementVector.y = idleTrackingTarget.position.y - transform.position.y;
                        movement = playerMovementVector * playerSpeed * .25f;
                    }
                }

                Debug.Log($"Velocity: {playerMovementVector},\tSpeed: {playerSpeed}");
                rb.AddForce(movement);
            }
            // Correct player position after hitting the barrier / switching Player objects:
            // playerVelocity.y = isNewPlayer ? (playerDefaultHeight - transform.position.y) * verticalResistance * playerSpeed : 0;

            // rb.velocity = playerMovementVector;
        }
    }

    public void OnCollisionEnter2D() {
        // Once the player object has touched the barrier, toggle the boolean for y-axis movement in mVelocity. (It will now always be 0 to prevent the Rigidbody from bouncing.)
        isNewPlayer = false;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Despawner") {
            if (this.enabled && this.rb.velocity.y <= 0) {
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