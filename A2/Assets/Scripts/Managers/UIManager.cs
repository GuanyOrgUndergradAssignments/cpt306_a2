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

    // controls how far the UIs are placed before the camera.
    public float uiCameraDistance = 20.0f;
    // controls the size of the UIs in the world space.
    public float uiWorldSpaceScale = 1.0f;
    // controls the orientation of the UIs in the world space.
    // A UI is either facing the camera or facing the other side.
    public bool uiWorldSpaceSide = true;
    // how much the UIs are rotated around their centers.
    public float uiWorldSpaceRotation = 0.0f;

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
        difficultyMenu.SetActive(false);
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
    /// In Unity VR, screen space UIs are not rendered.
    /// Instead, we must put UIs in world space as 3D objects.
    /// 
    /// To achieve the previous screen space effects, 
    /// we must make UIs move with the camera, and the best way to do that is to attach the UIs to the camera.
    /// </summary>
    private void attachMenusToCamera()
    {
        attachMenuToCamera(mainMenu);
        attachMenuToCamera(pauseMenu);
        attachMenuToCamera(gameOverMenu);
        attachMenuToCamera(inGameMenu);
        attachMenuToCamera(scoresMenu);
        attachMenuToCamera(optionsMenu);
        attachMenuToCamera(difficultyMenu);
}

    /// <summary>
    /// Helper for the previous method
    /// </summary>
    /// <param name="menu"></param>
    private void attachMenuToCamera(GameObject menu)
    {
        Camera cam = Game.gameSingleton.camMgr.getMainCamera();

        // attach to the camera.
        menu.GetComponent<Canvas>().worldCamera = cam;
        menu.transform.SetParent(cam.transform);

        RectTransform rectTransform = menu.GetComponent<RectTransform>();
        Utility.MyDebugAssert(rectTransform != null, "Code is incorrect. Fix it.");

        // location and rotation
        {
            // Each corner provides its world space value. The returned array of 4 vertices is clockwise.
            // It starts bottom left and rotates to top left, then top right, and finally bottom right.
            // Note that bottom left, for example, is an (x, y, z) vector with x being left and y being bottom.
            Vector3[] uiWorldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(uiWorldCorners);

            Vector3 bottomLeft = uiWorldCorners[0];
            Vector3 topRight = uiWorldCorners[2];
            float realUiWidth = Mathf.Abs(topRight.x - bottomLeft.x);
            float realUiHeight = Mathf.Abs(topRight.y - bottomLeft.y);

            // Unity applies rotation before translation.
            menu.transform.SetLocalPositionAndRotation
            (
                // match the ui center with the lookat vector.
                //new Vector3(.5f * realUiWidth, .5f * realUiHeight, uiCameraDistance),
                new Vector3(0.0f, 0.0f, uiCameraDistance),

                // By default Unity rotates around z first.
                // But we want to rotate around y first in this case.
                //
                // Let p = (x,y,z) be a point in 3D space (extended as a quaternion [0, x, y, z])
                // and let a be any valid rotation quaternion.
                // rotate p by using a is done by p' = a p a^{-1}.
                // Then, rotate it by b to get p'' = ba p a^{-1}b^{-1},
                // which means rotating by a and then by b
                // is equivalent to rotating by b*a
                Quaternion.Euler(0.0f, 0.0f, uiWorldSpaceRotation) * Quaternion.Euler(0.0f, uiWorldSpaceSide ? 0.0f : 180.0f, 0.0f)
            );
        }

        // size
        {
            // Need to scale through one of the two
            menu.transform.localScale = new Vector3(uiWorldSpaceScale, uiWorldSpaceScale, uiWorldSpaceScale);
            //rectTransform.localScale.Set(uiWorldSpaceScale, uiWorldSpaceScale, uiWorldSpaceScale);
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
        this.difficultyMenu = GameObject.Instantiate(difficultyMenu);

        Utility.MyDebugAssert(inGameMenu != null);
        Utility.MyDebugAssert(mainMenu != null);
        Utility.MyDebugAssert(pauseMenu != null);
        Utility.MyDebugAssert(gameOverMenu != null);
        Utility.MyDebugAssert(scoresMenu != null);
        Utility.MyDebugAssert(optionsMenu != null);
        Utility.MyDebugAssert(difficultyMenu != null);
    }

    private void Start()
    {
        // Attach them all to the camera
        attachMenusToCamera();

        // Bind menu elements callbacks
        bindMenuCallbacks();

        // hide all, and the in game menu
        // when I want to show one, call the corresponding method.
        hideAllUI();
        // main UI
        mainMenu.SetActive(true);
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
