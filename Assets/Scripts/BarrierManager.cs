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
    public static float Speed { get {return m_MovementSpeed; }}
    public static int ComponentCount { get {return m_PieceCount; }}
    public static float PieceScale { get {return m_PieceScale; }}

    public enum Shape { UNINITIALIZED = 0, CIRCLE = 1, TRIANGLE = 2, SQUARE = 3 };      // Used to determin current and future player objects to be spawned and their corresponding keyholes

    [SerializeField]
    private float m_SpawnSpeed = 4.0f;               // Interval (in seconds) in which barriers are spawned. m_SpawnTimer is reset to this value when it reaches 0.
    [SerializeField]
    private static float m_MovementSpeed = 100.0f;            // Speed with which each Barrier (and deactivated players) move
    [SerializeField]
    private static int m_PieceCount = 11;                   // Dictates how many barrier pieces comprise a barrier

    private RectTransform m_Container;               // Empty GameObject that is the parent to all spawned barriers
    private float m_SpawnTimer = 0f;                 // Time (in seconds) until the next barrier will spawn
    private static float m_PieceScale;                      // Ratio used to rescale the pieces of a barrier to remove any gaps between them 

    // Unity Methods:
    private void Awake() {
        m_Container = (RectTransform)transform.GetChild(0);
        m_PieceScale = m_Container.rect.width / m_PieceCount;      // This will always be relative to the camera of the camera due to the RectTransform's settings in m_Container
    }
    private void OnEnable() {
        // In the event there are barriers from the last game, clear them:
        for (int c = 0; c < m_Container.childCount; c++) {
            GameObject toBeDeleted = m_Container.GetChild(c).gameObject;
            if (toBeDeleted.tag == "Untagged")
                Destroy(toBeDeleted);
        }
    }

    private void Update() {
        // We only want to spawn a new Barrier if we're in-game
        if (GameManager.Instance.GameState == GameManager.GameStates.IN_GAME) {           
            if (m_SpawnTimer < 0) {
                m_SpawnTimer = m_SpawnSpeed;
                SpawnBarrierAndNextPlayer();
            }

            else
                m_SpawnTimer -= Time.deltaTime;
        }
    }


    // SpawnBarrier ():     Creates a new barrier using the barrier piece prefabs, creates the next player object, and attaches it to it
    private void SpawnBarrierAndNextPlayer() {
        // Create new barrier & player objects and attach the player to the barrier
        Barrier newBarrier = Barrier.CreateNewBarrier(m_Container);
        newBarrier.AttachPlayer(Player.CreateNewPlayer(newBarrier.Type));
    
        // Start new barrier (and player) downward movement
        newBarrier.Move();
    }
}
