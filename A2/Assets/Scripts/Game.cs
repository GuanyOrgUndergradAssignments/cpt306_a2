using System.Collections.Generic;
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
    // assigned in editor
    public ModelsManager modelMgr;
    // assigned in editor
    public UIManager uiMgr;

    // created inside
    public readonly Board board;

    // players
    IChessPlayer player1;
    IChessPlayer player2;

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
    public readonly Vector2Int player2Start = (int)Board.BOARD_LENGTH * Vector2Int.one;

    /*********************************** CTOR ***********************************/
    public Game()
    {
        // can only have one instance per game
        Utility.MyDebugAssert(gameSingleton == null);
        gameSingleton = this;

        // Create the managers that don't need prefabs
        stateMgr = new StateManager();

        // Create the board
        board = new Board(player1Start, player2Start);

        // players
        player1 = new HumanChessPlayer(Board.BoardPositionState.PLAYER1);
        player2 = new AIChessPlayer(Board.BoardPositionState.PLAYER2, (uint)difficulty);

        // all that can't be inited here are inited in Awake().
    }

    /*********************************** METHODS ***********************************/

    /// <summary>
    /// Called when the user clicks on start game on the main UI.
    /// </summary>
    public void startGame()
    {
        stateMgr.startGame();
        modelMgr.onGameStart(player1Start, player2Start);

        uiMgr.inGameMenu.SetActive(true);
    }
    
    /// <summary>
    /// Called inside Update()
    /// </summary>
    private void switchTurn(IChessPlayer nextPlayer)
    {
        stateMgr.switchTurn();
        nextPlayer.startMakingMove(board);
    }

    /*********************************** MONO ***********************************/

    /// <summary>
    /// Check if all prefabs are okay and init all that can only be inited here
    /// </summary>
    public void Awake()
    {
        uiMgr = GameObject.Instantiate(uiMgr);
        modelMgr = GameObject.Instantiate(modelMgr);
        Utility.MyDebugAssert(uiMgr != null, "check prefabs in editor.");
        Utility.MyDebugAssert(modelMgr != null, "check prefabs in editor.");
    }

    /// <summary>
    /// Now all things are ready. Prepare the game for the application start.
    /// </summary>
    public void Start()
    {
        // bg music
        {
            bgMusic = gameObject.GetComponent<AudioSource>();
            Utility.MyDebugAssert(bgMusic != null, "Should assign music.");

            // starts playing the bg music
            bgMusic.volume = AudioManager.musicStrength();
            bgMusic.Play();
        }

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
            if(board.getBoardState() == Board.BoardState.PLAYER1_WON || board.getBoardState() == Board.BoardState.PLAYER2_WON)
            {
                stateMgr.gameOver();
                uiMgr.gameOverMenu.SetActive(true);
            }
            else // the game is not over
            {
                // switch to the next player
                switchTurn(nextPlayer);
            }
        }
    }
}