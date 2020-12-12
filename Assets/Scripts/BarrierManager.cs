/*
    FILE:       BarrierManager.cs
    AUTHOR:     saphirako
    DATE:       20 OCT 2018

    DESCRIPTION:
    BarrierManager handles spawning, despawning, and movement of Barrier objects.

    LEGAL:
    Copyright © 2020 Saphirako
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarrierManager : MonoBehaviour {
    // ///////////////////////////////
    // Barrier piece prefabs: ////////
    // These contain the GameObjects used to create individual pieces of a barrier    
    // ///////////////////////////////
    [SerializeField]
    private GameObject m_PieceContainer;
    [SerializeField]
    private GameObject m_DefaultBarrier;
    [SerializeField]
    private GameObject m_CircleBarrier;
    [SerializeField]
    private GameObject m_TriangleBarrier;
    [SerializeField]
    private GameObject m_SquareBarrier;
    // ///////////////////////////////
    
    public enum Shape {CIRCLE = 0, TRIANGLE = 1, SQUARE = 2};      // Used to determin current and future player objects to be spawned and their corresponding keyholes
    public Shape MustInclude = Shape.CIRCLE;      // Represents what kind of barrier piece MUST be included in the next barrier in order to continue gameplay

    [SerializeField]
	private float m_SpawnSpeed = 3.0f;               // Interval (in seconds) in which barriers are spawned. m_SpawnTimer is reset to this value when it reaches 0.
	[SerializeField]
	private float m_MovementSpeed = 7.0f;            // Speed with which each Barrier (and deactivated players) move
    [SerializeField]
    private int m_PieceCount = 11;                   // Dictates how many barrier pieces comprise a barrier

    private RectTransform m_Container;               // Empty GameObject that is the parent to all spawned barriers
	private float m_SpawnTimer = 0f;                 // Time (in seconds) until the next barrier will spawn
    private float m_PieceScale;                      // Ratio used to rescale the pieces of a barrier to remove any gaps between them 

    // Unity Methods:
    private void Awake () {
        m_Container = (RectTransform)transform.GetChild (0);
        m_PieceScale = m_Container.rect.width / m_PieceCount;      // This will always be relative to the camera of the camera due to the RectTransform's settings in m_Container
    }
    private void OnEnable () {
        // In the event there are barriers from the last game, clear them:
        for (int c = 0; c < m_Container.childCount; c++) {
            GameObject toBeDeleted = m_Container.GetChild (c).gameObject;
            if (toBeDeleted.tag == "Untagged")
                Destroy (toBeDeleted);
        }
    }

    private void Update () {
        if (m_SpawnTimer < 0) {
			m_SpawnTimer = m_SpawnSpeed;
            SpawnBarrierAndNextPlayer ();
		}

		else
			m_SpawnTimer -= Time.deltaTime;
    }



    // SpawnBarrier ():     Creates a new barrier using the barrier piece prefabs, creates the next player object, and attaches it to it
    private void SpawnBarrierAndNextPlayer () {
        // We only want to actually spawn a new Barrier if we're in-game
        if (GameManager.Instance.GameState == GameManager.GameStates.IN_GAME) {
            // Create a new PieceContainter, make it a child of the container:
            GameObject newBarrier = Instantiate (m_PieceContainer, m_Container);

            // Create a reference that will be used when generating pieces of the barrier
            GameObject newPiece;

            // Find a place for the prefab to go that is within the bookends of the barrier
            int keyholeIndex = (int)Random.Range (1, m_PieceCount - 2);

            // Create the pieces of the barrier:
            for (int c = 0; c < m_PieceCount; c++) {
                if (c == keyholeIndex) {
                    // If c and keyholeIndex match, we need to instiate a piece that matches the type in MustInclude.
                    switch (MustInclude) {
                        case Shape.CIRCLE:
                            newPiece = Instantiate (m_CircleBarrier, newBarrier.transform);
                            break;

                        case Shape.SQUARE:
                            newPiece = Instantiate (m_SquareBarrier, newBarrier.transform);
                            break;
                            
                        case Shape.TRIANGLE:
                            newPiece = Instantiate (m_TriangleBarrier, newBarrier.transform);
                            break;

                        default:
                            Debug.LogError ("Failure creating a keyhole! Generating non-keyhole barrier piece.");
                            newPiece = Instantiate (m_DefaultBarrier, newBarrier.transform);
                            break;
                    }
                }

                else
                    newPiece = Instantiate (m_DefaultBarrier, newBarrier.transform);

                // Properly scale the new piece
                newPiece.transform.localScale = new Vector3 (m_PieceScale, m_PieceScale, 1);
            }

            // //////////////////////////////////////////////////////////////////
            // With the new barrier now created, we need to attach the new player object.
            // //////////////////////////////////////////////////////////////////
            // First, determine its shape:
            MustInclude = (Shape) ((int) Random.Range (0, 3));

            // Then, instantiate the correlating prefab:
            Transform newPlayer = GameManager.Instance.SpawnNewPlayer (MustInclude).transform;

            // Then, position the new player directly above the keyhole
            newPlayer.SetParent (newBarrier.transform.GetChild (keyholeIndex));
            newPlayer.localPosition = new Vector3 (0, 1.5f, 1);
            newPlayer.localScale = new Vector3 (1, 1, 1);

            // Finally, disable the PlayerScript
            newPlayer.gameObject.GetComponent<PlayerScript> ().enabled = false;
            // /////////////////////////////////////////////////////////////////////////////////////

            // Add a downward velocity to the newly created barrier:
            newBarrier.GetComponent<Rigidbody2D> ().velocity = newPlayer.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -m_MovementSpeed);
        }
    }
}
