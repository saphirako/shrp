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
    public static GameManager Instance;                 // Singleton to be accessed by any script within the game
    public enum GameStates { MENU = 0, IN_GAME = 1, GAME_OVER = 2 };
    public GameStates GameState = GameStates.IN_GAME;
    public int HighScore { get { return m_HighScore; } }
    public int Score { get { return m_CurrentScore; } }

    [SerializeField]
    private BarrierManager mBarrierManager;
    [SerializeField]
    private UIManager m_UIManager;
    public Transform InitialSpawn;

    private int m_CurrentScore;
    private int m_HighScore;


    // Unity Methods:
    public void Awake() {
        if (Instance == null) {
            Instance = this;
            Player.InitializeResources();           // Load the prefabs into the Player scipt
            Barrier.InitializeResources();          // Load the prefabs into the Barrier script
            DontDestroyOnLoad(gameObject);
        }

        else {
            if (Instance != this) {
                Destroy(gameObject);
                return;
            }
        }
    }


    // NewGame ():		Spawns a first player, advises UI Manager, and sets the GameState to IN_GAME
    public void NewGame() {
        m_UIManager.HideMainMenu();
        Player.Current = Player.CreateNewPlayer((BarrierManager.Shape)((int)Random.Range(1, 4)), true);
        GameState = GameStates.IN_GAME;
    }


    // GameOver ():		Sets the GameState to GAME_OVER and either shows the GameOver screen or shows the main menu, depending on if it's a "ScriptGameOver" (end of the game round) or a "GraphicGameOver" (called by button on GameOver screen which tells us to return to the main menu)
    public void GameOver(bool isScriptGameOver) {
        GameState = GameStates.GAME_OVER;

        if (isScriptGameOver)
            m_UIManager.ShowGameOverScreen();

        else {
            m_UIManager.HideGameOverScreen();
            m_UIManager.ShowMainMenu();
        }
    }
}