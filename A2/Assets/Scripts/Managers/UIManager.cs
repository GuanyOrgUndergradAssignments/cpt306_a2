using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all UIs in the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    // in Game HUD
    public GameObject inGameMenu;
    public GameObject creditsMenu;
    public GameObject rankingsMenu;
    public GameObject optionsMenu;

    /// <summary>
    /// Hide all except the in game menu
    /// </summary>
    public void hideAllExceptInGameUI()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        creditsMenu.SetActive(false);
        rankingsMenu.SetActive(false);
        optionsMenu.SetActive(false);
        // inGameMenu.SetActive(false);
    }

    public void hideAllUI()
    {
        hideAllExceptInGameUI();
        inGameMenu.SetActive(false);
    }

    /*********************************** Mono ***********************************/

    /// <summary>
    /// When the UI manager is awake, it instantiates all UI prefabs
    /// and hide all of them.
    /// </summary>
    private void Awake()
    {
        // should be available before all.
        Utility.MyDebugAssert(Game.gameSingleton != null);

        // spawn all menus.
        // inGameMenu is spawned before pauseMenu so that if both are shown,
        // then pauseMenu overlapps the in game one.
        this.inGameMenu = GameObject.Instantiate(inGameMenu);
        this.mainMenu = GameObject.Instantiate(mainMenu);
        this.pauseMenu = GameObject.Instantiate(pauseMenu);
        this.gameOverMenu = GameObject.Instantiate(gameOverMenu);
        this.creditsMenu = GameObject.Instantiate(creditsMenu);
        this.rankingsMenu = GameObject.Instantiate(rankingsMenu);
        this.optionsMenu = GameObject.Instantiate(optionsMenu);

        Utility.MyDebugAssert(inGameMenu != null);
        Utility.MyDebugAssert(mainMenu != null);
        Utility.MyDebugAssert(pauseMenu != null);
        Utility.MyDebugAssert(gameOverMenu != null);
        Utility.MyDebugAssert(creditsMenu != null);
        Utility.MyDebugAssert(rankingsMenu != null);
        Utility.MyDebugAssert(optionsMenu != null);

        // hide all, and the in game menu
        // when I want to show one, call the corresponding method.
        hideAllUI();
    }

    public void OnDestroy()
    {
        GameObject.Destroy(mainMenu.gameObject);
        GameObject.Destroy(pauseMenu.gameObject);
        GameObject.Destroy(gameOverMenu.gameObject);
        GameObject.Destroy(creditsMenu.gameObject);
        GameObject.Destroy(rankingsMenu.gameObject);
        GameObject.Destroy(optionsMenu.gameObject);
        GameObject.Destroy(inGameMenu.gameObject);
    }

}
