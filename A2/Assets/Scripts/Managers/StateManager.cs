using System;
using System.Collections.Generic;
using System.IO;

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
        // when the game is running
        RUNNING,
        // when the player has won
        VICTORY,
        // when the player has lost
        GAME_OVER
    }

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

    private State state;
    private int score;
    // Set only in options UI. The state mananger doesn't set it.
    public Difficulty difficulty;

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
    /// From MU to R
    /// </summary>
    public void startGame()
    {
        Utility.MyDebugAssert(state == State.MAIN_UI);

        score = 0;
        state = State.RUNNING;
    }

    /// <summary>
    /// From R to P
    /// </summary>
    public void pause()
    {
        Utility.MyDebugAssert(state == State.RUNNING);
        state = State.PAUSED;
    }

    /// <summary>
    /// From P to R
    /// </summary>
    public void resume()
    {
        Utility.MyDebugAssert(state == State.PAUSED);
        state = State.RUNNING;
    }

    /// <summary>
    /// From P, V, GO to MU
    /// </summary>
    public void goHome()
    {
        // can be called when
        // V, P, N, GO
        Utility.MyDebugAssert(state != State.MAIN_UI && state != State.RUNNING);

        state = State.MAIN_UI;
    }

    /// <summary>
    /// From R to V.
    /// Saves the score to the local file.
    /// </summary>
    public void win()
    {
        Utility.MyDebugAssert(state == State.RUNNING);
        state = State.VICTORY;

        // save score to file.
        saveScoresToFile();
    }

    /// <summary>
    /// From R to GO
    /// </summary>
    public void gameOver()
    {
        Utility.MyDebugAssert(state == State.RUNNING);
        state = State.GAME_OVER;
    }

}
