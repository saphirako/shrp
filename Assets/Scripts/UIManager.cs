/*
    FILE:       UIManager.cs
    DATE:       19 NOV. 2018
    AUHTOR:     saphirako

    DESCRIPTION:
    UIManager controls all UI elements in the game including player score, menu's, and pop-ups.

    LEGAL:
    Copyright © 2020 Saphirako
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private Button m_MainMenuButton;            // Button component of the 'Main Menu Scren' GameObject that calls GameManager.NewGame ()
    [SerializeField]
    private Button m_GameOverButton;            // Button component of the 'Game Over Screen' GameObject that calls GameManager.GameOver ()
    [SerializeField]
    private TextMeshProUGUI m_GameOverScore;    // TextMeshPro UGUI component of the 'Game Over Screen' GameObject that shows GameManager.Score at the end of a session
    [SerializeField]
    private Animator m_MainMenuAnimator;        // Animator component of the 'Main Menu Screen' GameObject responsible for all Main Menu screen animations
    [SerializeField]
    private Animator m_GameOverAnimator;        // Animator component of the 'Game Over Screen' GameObject responsible for all Game Over screen animations


    // ShowMainMenu ():    Activates the animation for making the main menu appear and enables the button within the Main Menu GameObject
    public void ShowMainMenu () {
        m_MainMenuAnimator.ResetTrigger ("Slide Out");
        m_MainMenuAnimator.SetTrigger ("Slide In");
    }

    // HideMainMenu ():    Activates the animation for hiding the menu and disables the button withing the Main Menu GameObject to prevent unwanted clicks
    public void HideMainMenu () {
        m_MainMenuAnimator.ResetTrigger ("Slide In");
        m_MainMenuAnimator.SetTrigger ("Slide Out");
    }

    // ShowGameOverScreen ():   Activates the 'Game Over Visible' animation, enables the Button component on the 'Game Over Screen' GameObject, and updates the player score TextMeshPro - Text object on the GOS 
    public void ShowGameOverScreen (bool isHighScore = false) {
        m_GameOverButton.enabled = true;
        m_GameOverScore.text = System.Convert.ToString (GameManager.Instance.Score);

        m_GameOverAnimator.ResetTrigger ("Hide Screen");    
        m_GameOverAnimator.SetTrigger ("Show Screen");
        m_GameOverAnimator.SetBool ("High Score", isHighScore);
    }

    // HideGameOverScreen ():   Resets the Animation Conditions set in ShowGameOverScreen (), disables the Button component on the 'Game Over Screen' GameObject, and activates the 'Game Over Hide' animation
    public void HideGameOverScreen () {
        m_GameOverButton.enabled = false;
        m_GameOverAnimator.ResetTrigger ("Show Screen");
        m_GameOverAnimator.SetBool ("High Score", false);
        m_GameOverAnimator.SetTrigger ("Hide Screen");    
    }
}