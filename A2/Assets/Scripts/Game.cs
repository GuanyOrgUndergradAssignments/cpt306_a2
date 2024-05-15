using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static StateManager;

/// <summary>
/// The big game object is the mediator 
/// which every other component in the game talks to
/// </summary>
public sealed class Game : MonoBehaviour
{
    /*********************************** STATIC ***********************************/

    /// <summary>
    /// Difficulty of the game.
    /// </summary>
    public enum Difficulty
    {
        EASY = 0,
        NORMAL = 1,
        HARD = 2,
        NUMBER_DIFFICULTIES = 3
    }

    /*********************************** FIELDS ***********************************/
    // For convenience, I expose all fields as public.
    // Do not assign to any of them outside of Game!

    // created inside
    public readonly StateManager stateMgr;
    // assigned in the editor as a prefab
    public ModelsManager modelMgr;
    // assigned in the editor as a prefab
    public UIManager uiMgr;
    // assigned in the editor as a prefab
    public CameraManager camMgr;

    // created inside
    public readonly Board board;

    // players
    IChessPlayer player1;
    IChessPlayer player2;
    // Because it needs to use some Unity functions,
    // a player is a mono script. Therefore, they have to be created via prefabs.
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    // background music.
    // attached as a component to this instance.
    AudioSource bgMusic;

    // Will always be available before all's ctor
    // (because Game creates all, and in its ctor, the singleton var is assigned first).
    // Game acts as the mediator. All objects talk to it.
    public static Game gameSingleton = null;

    // Set only in options UI. The state mananger doesn't set it.
    public Difficulty difficulty;

    // start pawn positions
    public readonly Vector2Int player1Start = Vector2Int.zero;
    public readonly Vector2Int player2Start = (int)(Board.BOARD_LENGTH - 1) * Vector2Int.one;

    /*********************************** CTOR ***********************************/
    public Game()
    {
        // Create the managers that don't need prefabs
        stateMgr = new StateManager();

        // Create the board
        board = new Board(player1Start, player2Start);

        // all that can't be inited here are inited in Awake().
    }

    /*********************************** METHODS ***********************************/

    /// <summary>
    /// Called when the user clicks on start game on the main UI.
    /// </summary>
    public void startGame()
    {
        Debug.Log("Game started.");

        // state
        stateMgr.startGame();

        // models
        modelMgr.onGameStart(player1Start, player2Start);
        
        // UI
        uiMgr.mainMenu.SetActive(false);
        uiMgr.inGameMenu.SetActive(true);

        // the first move
        player1.startMakingMove(board);
    }

    /// <summary>
    /// called when the player wants to pause the game during gameplay
    /// </summary>
    public void pauseGame()
    {
        // stateMgr asserts the state already.

        Time.timeScale = 0.0f;
        stateMgr.pause();
        uiMgr.pauseMenu.SetActive(true);
    }

    /// <summary>
    /// called when the player resumes a paused game
    /// </summary>
    public void resumeGame()
    {
        // stateMgr asserts the state already.

        Time.timeScale = 1.0f;
        stateMgr.resume();
        uiMgr.hideAllExceptInGameUI();
    }

    /// <summary>
    /// Called inside Update()
    /// </summary>
    private void switchTurn(IChessPlayer nextPlayer)
    {
        stateMgr.switchTurn();
        nextPlayer.startMakingMove(board);
    }

    /// <summary>
    /// Called whenever the user wants to go back to main menu.
    /// </summary>
    public void goHome()
    {
        // stateMgr asserts the state already.

        // reset the players.
        player1.reset();
        player2.reset();

        // in case it was paused
        Time.timeScale = 1.0f;

        // destroy all pawns
        modelMgr.clear();
        // UI
        uiMgr.hideAllUI();

        // state change
        stateMgr.goHome();

        // brings up the main ui
        uiMgr.mainMenu.SetActive(true);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void exit()
    {
        // can only exit from the main ui.
        Utility.MyDebugAssert(stateMgr.getState() == StateManager.State.MAIN_UI);

        // destroy all game objects
        // including self
        {
            // menus are destroyed by the Manager's OnDestroy()
            GameObject.Destroy(uiMgr.gameObject);
            // board and pawns are destroyed by the Manager's OnDestroy()
            GameObject.Destroy(modelMgr.gameObject);
            GameObject.Destroy(camMgr.gameObject);
            GameObject.Destroy(this.gameObject);
        }

        // exit the game.
        Application.Quit(0);
    }

    /// <summary>
    /// Called by myself in Update()
    /// </summary>
    private void onGameOver()
    {
        stateMgr.gameOver();
        uiMgr.onGameOver(board.getBoardState());
    }

    /*********************************** MONO ***********************************/

    /// <summary>
    /// Check if all prefabs are okay and init all that can only be inited here
    /// </summary>
    public void Awake()
    {
        // can only have one instance per game
        Utility.MyDebugAssert(gameSingleton == null);
        gameSingleton = this;

        uiMgr = GameObject.Instantiate(uiMgr);
        modelMgr = GameObject.Instantiate(modelMgr);
        camMgr = GameObject.Instantiate(camMgr);
        Utility.MyDebugAssert(uiMgr != null, "check prefabs in editor.");
        Utility.MyDebugAssert(modelMgr != null, "check prefabs in editor.");
        Utility.MyDebugAssert(camMgr != null, "check prefabs in editor.");

        // players
        Utility.MyDebugAssert(player1Prefab != null, "check prefabs in editor.");
        Utility.MyDebugAssert(player2Prefab != null, "check prefabs in editor.");
        var p1Obj = GameObject.Instantiate(player1Prefab);
        var p2Obj = GameObject.Instantiate(player2Prefab);
        player1 = p1Obj.GetComponent<HumanChessPlayer>() as IChessPlayer;
        player2 = p2Obj.GetComponent<AIChessPlayer>() as IChessPlayer;
        Utility.MyDebugAssert(player1 != null, "Did I forget to attach scripts?");
        Utility.MyDebugAssert(player2 != null, "Did I forget to attach scripts?");
    }

    /// <summary>
    /// Now all things are ready. Prepare the game for the application start.
    /// </summary>
    public void Start()
    {
        // bg music
        // {
        //     bgMusic = gameObject.GetComponent<AudioSource>();
        //     Utility.MyDebugAssert(bgMusic != null, "Should assign music.");

        //     // starts playing the bg music
        //     bgMusic.volume = AudioManager.musicStrength();
        //     bgMusic.Play();
        // }

        // main UI
        uiMgr.mainMenu.SetActive(true);
    }

    /// <summary>
    /// If it's outside of a player's turn, then do nothing.
    /// Otherwise
    ///     1. p = current player
    ///     2. if p has finished making move, then 
    ///         - modelMgr.onMoveMad()
    ///         - if the game is over, then go to the corresponding state
    ///         - otherwise, switch turn.
    ///     
    /// </summary>
    public void Update()
    {
        var state = stateMgr.getState();
        IChessPlayer currentPlayer = null;
        IChessPlayer nextPlayer = null;

        if (state == State.PLAYER1)
        {
            currentPlayer = player1;
            nextPlayer = player2;
        }
        else if (state == State.PLAYER2)
        {
            currentPlayer = player2;
            nextPlayer = player1;
        }
        // Outside of a player's turn
        else
        {
            return;
        }

        if (!currentPlayer.hasFinishedMakingMove())
        {
            return;
        }
        else // finished making move
        {
            // let the move have effect on the board
            var move = currentPlayer.getMove();
            var changedPawns = board.makeMove(move);

            // sync the move to the models and play the effects
            modelMgr.onMoveMade(move, changedPawns, move.getPlayer());

            // check if the game is over after the move
            if(board.getBoardState() == Board.BoardState.PLAYER1_WON || 
                board.getBoardState() == Board.BoardState.PLAYER2_WON || 
                board.getBoardState() == Board.BoardState.DRAW)
            {
                onGameOver();
            }
            else // the game is not over
            {
                // switch to the next player
                switchTurn(nextPlayer);
            }
        }
    }
}