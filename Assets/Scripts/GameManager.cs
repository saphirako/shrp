/*
	FILE:		GameManager.cs
	AUHTOR:		saphirako
	DATE:		19 OCT 2018

	DESCRIPTION:
	This file comprises the main runtime processes and links to the other managers
	of the game.

	LEGAL:
	Copyright © 2020 Saphirako
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;					// Singleton to be accessed by any script within the game
	public enum GameStates {MENU = 0, IN_GAME = 1, GAME_OVER = 2};
	public GameStates GameState = GameStates.IN_GAME; 
	public int HighScore { get { return m_HighScore;} }
	public int Score { get { return m_CurrentScore;} }

	[SerializeField]
	private PlayerScript mPlayer;
	[SerializeField]
	private BarrierManager mBarrierManager;
	[SerializeField]
	private UIManager m_UIManager;
	[SerializeField]
	private Transform mFirstPlayerSpawnPoint;

	private int m_CurrentScore;
	private int m_HighScore;
	// ///////////////////////////////
    // Player prefabs:  //////////////
    // These contain the GameObjects used to create player objects    
    // ///////////////////////////////
    [SerializeField]
    private GameObject m_CirclePlayer;
    [SerializeField]
    private GameObject m_TrianglePlayer;
    [SerializeField]
    private GameObject m_SquarePlayer;
    // ///////////////////////////////

	// Unity Methods:
	public void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}

		else {
			if (Instance != this) {
				Destroy (gameObject);
				return;
			}
		}
	}

	// SetNewPlayer ():		Sets mPlayer to the argued PlayerScript and rename its GameObject to 'Player'
	public void SetNewPlayer (PlayerScript newPlayer) {
		mPlayer = newPlayer;
		mPlayer.gameObject.name = "Player";
	}

	// SpawnNewPlayer ():		Instantiates a new player prefab based on the provided Shape. The attached boolean is used to determine whether this is the first player of the game.
	public GameObject SpawnNewPlayer (BarrierManager.Shape requiredShape, bool isAFirstPlayer = false) {
		GameObject newPlayer;

		switch (requiredShape) {
			case BarrierManager.Shape.CIRCLE:
				newPlayer = Instantiate (m_CirclePlayer);
				break;
			
			case BarrierManager.Shape.TRIANGLE:
				newPlayer = Instantiate (m_TrianglePlayer);
				break;
			
			case BarrierManager.Shape.SQUARE:
				newPlayer = Instantiate (m_SquarePlayer);
				break;

			default:
				Debug.LogError ("Error creating a new player. Defaulting to Shape.CIRCLE.");
				newPlayer = Instantiate (m_CirclePlayer);
				break;
		}

		// If this is the first player of the game, we need to move it to the default position for first players and update mPlayer
		if (isAFirstPlayer) {
			newPlayer.transform.SetPositionAndRotation (mFirstPlayerSpawnPoint.position, mFirstPlayerSpawnPoint.rotation);
			SetNewPlayer (newPlayer.GetComponent<PlayerScript> ());
		}

		return newPlayer;
	}

	// NewGame ():		Spawns a first player, advises UI Manager, and sets the GameState to IN_GAME
	public void NewGame () {
		m_UIManager.HideMainMenu ();
		SpawnNewPlayer (mBarrierManager.MustInclude, true);
		GameState = GameStates.IN_GAME;
	}


	// GameOver ():		Sets the GameState to GAME_OVER and either shows the GameOver screen or shows the main menu, depending on if it's a "ScriptGameOver" (end of the game round) or a "GraphicGameOver" (called by button on GameOver screen which tells us to return to the main menu)
	public void GameOver (bool isScriptGameOver) {
		GameState = GameStates.GAME_OVER;
	
		if (isScriptGameOver)
			m_UIManager.ShowGameOverScreen ();

		else {
			m_UIManager.HideGameOverScreen ();
			m_UIManager.ShowMainMenu ();
		}
	}
}