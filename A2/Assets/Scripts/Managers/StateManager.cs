using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

/// <summary>
/// The state manager manages the state of the game.
/// It is a singleton class.
/// 
/// Its various methods provide means of state transitions.
/// </summary>
public class StateManager
{
    /*********************************** STATES ***********************************/
    /// <summary>
    /// States of the game
    /// </summary>
    public enum State
    {
        // when the game is at the main UI
        MAIN_UI,
        // when the game is paused
        PAUSED,
        // when player1 is taking the turn
        PLAYER1,
        // when player2 is taking the turn
        PLAYER2,
        // when the game is over.
        // P1 may win, P2 may win, or they may have a draw.
        GAME_OVER
    }

    /*********************************** CTOR ***********************************/
    public StateManager()
    {
        state = State.MAIN_UI;
        score = 0;
    }

    /*********************************** FIELDS ***********************************/

    private State state;
    private State whichPlayerBeforePaused = State.PLAYER1;
    private int score;

    public const String SCORE_FILE_PATH = "game_scores.txt";
    // score...datetime
    public const String SCORE_FILE_LINE_FMT = "{0}...{1}";

    /*********************************** OBSERVERS ***********************************/
    public State getState() { return state; }
    public int getScore() { return score; }
    public void addScore(int s)
    {
        Utility.MyDebugAssert(s > 0, "cannot add non-positive score");
        score += s;
    }

    /// <summary>
    /// Saves score to SCORE_FILE_PATH as a new line
    /// in this format: score...datetime
    /// </summary>
    private void saveScoresToFile()
    {
        String scoreLine = String.Format(SCORE_FILE_LINE_FMT, score, DateTime.Now);
        if (!File.Exists(SCORE_FILE_PATH))
        {
            // Create the file to write the score line to.
            using (StreamWriter sw = File.CreateText(SCORE_FILE_PATH))
            {
                sw.WriteLine(scoreLine);
            }
        }
        else
        {
            // append the score line to the existing file
            File.AppendAllLines(SCORE_FILE_PATH, new List<String> { scoreLine });
        }
    }

    /*********************************** STATE TRANSITIONS ***********************************/
    // states are abbreviated in the following comments.

    /// <summary>
    /// From MU to P1
    /// </summary>
    public void startGame()
    {
        Utility.MyDebugAssert(state == State.MAIN_UI);

        score = 0;
        state = State.PLAYER1;
    }

    /// <summary>
    /// P1 to P2, or P2 to P1
    /// </summary>
    public void switchTurn()
    {
        if(state == State.PLAYER1)
        {
            state = State.PLAYER2;
        }
        else if(state == State.PLAYER2)
        {
            state = State.PLAYER1;
        }
        else
        {
            Utility.MyDebugAssert(false, "Bad state.");
        }
    }

    /// <summary>
    /// From P1,P2 to P
    /// </summary>
    public void pause()
    {
        Utility.MyDebugAssert(state == State.PLAYER1 || state == State.PLAYER2);
        whichPlayerBeforePaused = state;
        state = State.PAUSED;

    }

    /// <summary>
    /// From P to P1,P2
    /// </summary>
    public void resume()
    {
        Utility.MyDebugAssert(state == State.PAUSED);
        Utility.MyDebugAssert(whichPlayerBeforePaused == State.PLAYER1 || whichPlayerBeforePaused == State.PLAYER2);
        state = whichPlayerBeforePaused;
    }

    /// <summary>
    /// From P, GO to MU
    /// </summary>
    public void goHome()
    {
        // can be called when
        // GO, P
        Utility.MyDebugAssert(state == State.GAME_OVER || state == State.PAUSED);

        state = State.MAIN_UI;
    }

    /// <summary>
    /// From P1,P2 to GO
    /// </summary>
    public void gameOver()
    {
        Utility.MyDebugAssert(state == State.PLAYER1 || state == State.PLAYER2);
        state = State.GAME_OVER;
    }

}
