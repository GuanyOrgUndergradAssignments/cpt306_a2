using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UIs in the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    /*********************************** FIELDS ***********************************/

    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    // in Game HUD
    public GameObject inGameMenu;
    public GameObject scoresMenu;
    public GameObject optionsMenu;
    // Trash Unity's Dropdown has stupid bugs.
    // Can't use that. Instead, create a menu myself.
    public GameObject difficultyMenu;

    // other elements that need to be stored (callbacks don't have parameters)
    private Slider[] optionsMenuSliders;

    /*********************************** PUBLIC METHODS ***********************************/

    /// <summary>
    /// Hide all except the in game menu
    /// </summary>
    public void hideAllExceptInGameUI()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        scoresMenu.SetActive(false);
        optionsMenu.SetActive(false);
        // inGameMenu.SetActive(false);
    }

    public void hideAllUI()
    {
        hideAllExceptInGameUI();
        inGameMenu.SetActive(false);
    }

    /*********************************** UI DATA BINDING ***********************************/

    // See https://docs.unity3d.com/Manual/UIE-Binding.html

    /// <summary>
    /// Score:{0}
    /// </summary>
    public string scoreStrProp
    {
        get => String.Format("Score:{0}", Game.gameSingleton.stateMgr.getScore());
    }

    /// <summary>
    /// Time:{0} seconds
    /// </summary>
    public string timeStrProp
    {
        get => String.Format("Time:{0} seconds", UnityEngine.Time.timeSinceLevelLoad);
    }

    /// <summary>
    /// "Mine:{0}, Opponent:{1}"
    /// </summary>
    public string numberPawnsProp
    {
        get
        {
            var boardStats = Game.gameSingleton.board.getBoardStatistics();
            return String.Format("Mine:{0}, Opponent:{1}", boardStats.y, boardStats.z);
        }
    }

    /*********************************** PRIVATE HELPERS ***********************************/

    public void onStartGameClicked()
    {
        Game.gameSingleton.startGame();
    }
    public void onScoresClicked()
    {
        scoresMenu.SetActive(true);
    }
    public void onOptionsClicked()
    {
        optionsMenu.SetActive(true);
    }
    public void onExitGameClicked()
    {
        Game.gameSingleton.exit();
    }
    public void onResumeGameClicked()
    {
        Game.gameSingleton.resumeGame();
    }
    public void onHomeClicked()
    {
        Game.gameSingleton.goHome();
    }
    /// <summary>
    /// Bring up the game over menu
    /// and set up the corresponding text
    /// </summary>
    /// <param name="whoWon"></param>
    public void onGameOver(Board.BoardState whoWon)
    {
        var texts = gameOverMenu.GetComponentsInChildren<TMP_Text>();
        Utility.MyDebugAssert(texts.Length == 2, "Should have two texts, one the title, another the button text.");
        var titleText = texts[0];

        if (whoWon == Board.BoardState.PLAYER1_WON)
        {
            titleText.text = "You have won.";
        }
        else if (whoWon == Board.BoardState.PLAYER2_WON)
        {
            titleText.text = "You have lost.";
        }
        else if (whoWon == Board.BoardState.DRAW)
        {
            titleText.text = "Game draw (neither has won).";
        }
        else // impossible
        {
            Utility.MyDebugAssert(false, "game over called when playing.");
        }
    }

    /// <summary>
    /// Helper called inside Awake()
    /// to bind all callbacks to the menus
    /// </summary>
    private void bindMenuCallbacks()
    {
        // https://docs.unity3d.com/Manual/InspectorOptions.html#reordering-components
        // states that
        // The component order you apply in the Inspector is
        // the same order that you need to use when you query components in your scripts.

#if UNITY_EDITOR

        // main menu buttons
        {
            var btns = mainMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 4, "check the menu");

            // start btn
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, onStartGameClicked);
            //btns[0].onClick.AddListener(onStartGameClicked);
            // scores btn
            UnityEventTools.AddVoidPersistentListener(btns[1].onClick, onScoresClicked);
            // options btn
            UnityEventTools.AddVoidPersistentListener(btns[2].onClick, onOptionsClicked);
            // exit btn
            UnityEventTools.AddVoidPersistentListener(btns[3].onClick, onExitGameClicked);
        }

        // pause menu buttons
        {
            var btns = pauseMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 2, "check the menu");

            // resume btn
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, onResumeGameClicked);
            // home btn
            UnityEventTools.AddVoidPersistentListener(btns[1].onClick, onHomeClicked);
        }

        // gameover buttons
        {
            var btns = gameOverMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 1, "check the menu");

            // home btn
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, onHomeClicked);
        }

        // scoresMenu buttons
        {
            var btns = scoresMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 1, "check the menu");

            // go back btn
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, () => scoresMenu.SetActive(false));
        }

        // options menu sliders and button
        {
            optionsMenuSliders = optionsMenu.GetComponentsInChildren<Slider>();
            Utility.MyDebugAssert(optionsMenuSliders.Length == 3, "check the menu");

            // master volume slider
            UnityEventTools.AddVoidPersistentListener
            (
                optionsMenuSliders[0].onValueChanged,
                () => AudioManager.masterVolume = optionsMenuSliders[0].value
            );
            // music volume slider
            UnityEventTools.AddVoidPersistentListener
            (
                optionsMenuSliders[1].onValueChanged,
                () => AudioManager.musicVolume = optionsMenuSliders[1].value
            );
            // effects volume slider
            UnityEventTools.AddVoidPersistentListener
            (
                optionsMenuSliders[2].onValueChanged,
                () => AudioManager.effectsVolume = optionsMenuSliders[2].value
            );

            var btns = optionsMenu.GetComponentsInChildren<Button>();
            Utility.MyDebugAssert(btns.Length == 2, "check the menu");
            // difficulty change button
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, () => difficultyMenu.SetActive(true));
            // go back button
            UnityEventTools.AddVoidPersistentListener(btns[1].onClick, () => optionsMenu.SetActive(false));
        }

        // difficulty menu buttons.
        {
            var btns = difficultyMenu.GetComponentsInChildren<Button>();
            Utility.MyDebugAssert(btns.Length == 3, "check the menu");
            // easy
            UnityEventTools.AddVoidPersistentListener(btns[0].onClick, () =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.EASY;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
            // normal
            UnityEventTools.AddVoidPersistentListener(btns[1].onClick, () =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.NORMAL;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
            // hard
            UnityEventTools.AddVoidPersistentListener(btns[2].onClick, () =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.HARD;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
        }
#else
        // main menu buttons
        {
            var btns = mainMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 4, "check the menu");

            // start btn
            btns[0].onClick.AddListener(onStartGameClicked);
            // ranking btn
            btns[1].onClick.AddListener(onScoresClicked);
            // options btn
            btns[2].onClick.AddListener(onOptionsClicked);
            // exit btn
            btns[3].onClick.AddListener(onExitGameClicked);
        }

        // pause menu buttons
        {
            var btns = pauseMenu.GetComponentsInChildren<Button>();

            Utility.MyDebugAssert(btns.Length == 2, "check the menu");

            // resume btn
            btns[0].onClick.AddListener(onResumeGameClicked);
            // home btn
            btns[1].onClick.AddListener(onHomeClicked);
        }

        // gameover buttons
        {
            var btns = gameOverMenu.GetComponentsInChildren<Button>();
            // home btn
            btns[0].onClick.AddListener(onHomeClicked);
        }

        // scores menu buttons
        {
            var btns = scoresMenu.GetComponentsInChildren<Button>();
            // go back btn
            btns[0].onClick.AddListener(() => scoresMenu.SetActive(false));
        }

        // options menu sliders and button
        {
            optionsMenuSliders = optionsMenu.GetComponentsInChildren<Slider>();
            Utility.MyDebugAssert(optionsMenuSliders.Length == 3);

            // master volume slider
            optionsMenuSliders[0].onValueChanged.AddListener
            (
                (value) => AudioManager.masterVolume = value
            );
            // music volume slider
            optionsMenuSliders[1].onValueChanged.AddListener
            (
                (value) => AudioManager.musicVolume = value
            );
            // effects volume slider
            optionsMenuSliders[2].onValueChanged.AddListener
            (
                (value) => AudioManager.effectsVolume = value
            );

            var btns = optionsMenu.GetComponentsInChildren<Button>();
            Utility.MyDebugAssert(btns.Length == 2);
            // difficulty change button
            btns[0].onClick.AddListener(() => difficultyMenu.SetActive(true));
            // go back button
            btns[1].onClick.AddListener(() => optionsMenu.SetActive(false));
        }

        // difficulty menu buttons.
        {
            var btns = difficultyMenu.GetComponentsInChildren<Button>();
            Utility.MyDebugAssert(btns.Length == 3);
            // easy
            btns[0].onClick.AddListener(() =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.EASY;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
            // normal
            btns[1].onClick.AddListener(() =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.NORMAL;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
            // hard
            btns[2].onClick.AddListener(() =>
            {
                Game.gameSingleton.difficulty = Game.Difficulty.HARD;
                difficultyMenu.SetActive(false);
                updateOptionsMenu();
            });
        }
#endif
    }

    private void updateOptionsMenu()
    {
        // Set difficulty text
        var btns = optionsMenu.GetComponentsInChildren<Button>();
        btns[0].GetComponentInChildren<TMP_Text>().text =
            "Current Difficulty: " +
            Game.gameSingleton.difficulty.ToString() +
            "\nClick to CHANGE";
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
        this.scoresMenu = GameObject.Instantiate(scoresMenu);
        this.optionsMenu = GameObject.Instantiate(optionsMenu);

        Utility.MyDebugAssert(inGameMenu != null);
        Utility.MyDebugAssert(mainMenu != null);
        Utility.MyDebugAssert(pauseMenu != null);
        Utility.MyDebugAssert(gameOverMenu != null);
        Utility.MyDebugAssert(scoresMenu != null);
        Utility.MyDebugAssert(optionsMenu != null);

        // Bind menu elements callbacks
        bindMenuCallbacks();

        // hide all, and the in game menu
        // when I want to show one, call the corresponding method.
        hideAllUI();
    }

    /// <summary>
    /// Update the in game HUD per frame
    /// </summary>
    private void Update()
    {
        // when the game is running
        if 
        (
            Game.gameSingleton.stateMgr.getState() == StateManager.State.PLAYER1 ||
            Game.gameSingleton.stateMgr.getState() == StateManager.State.PLAYER2
        )
        {
            // update the in game menu's texts
            // if that is active
            if (inGameMenu.activeSelf)
            {
                var texts = inGameMenu.GetComponentsInChildren<TMP_Text>();
                Utility.MyDebugAssert(texts.Length == 3);

                // 0. time text
                texts[0].text = timeStrProp;
                // 1. score text
                texts[1].text = scoreStrProp;
                // 2. number of pawns text
                texts[2].text = numberPawnsProp;
            }
        }
    }

    public void OnDestroy()
    {
        GameObject.Destroy(mainMenu.gameObject);
        GameObject.Destroy(pauseMenu.gameObject);
        GameObject.Destroy(gameOverMenu.gameObject);
        GameObject.Destroy(scoresMenu.gameObject);
        GameObject.Destroy(optionsMenu.gameObject);
        GameObject.Destroy(inGameMenu.gameObject);
    }

}
