using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a human player.
/// </summary>
public class HumanChessPlayer : ConcreteChessPlayer
{
    /********************************** FIELDS ************************************/
    private bool isMoving = false;
    private Coroutine moveCoro = null;

    /********************************** CTOR ************************************/

    public HumanChessPlayer(Board.BoardPositionState side) : base(side)
    {
    }

    /********************************** FROM ConcreteChessPlayer ************************************/

    public override bool hasFinishedMakingMove()
    {
        return !isMoving;
    }

    /// <summary>
    /// Cancel the move
    /// </summary>
    public override void reset()
    {
        isMoving = false;

        // stop the move coroutine.
        if(moveCoro != null)
        {
            this.StopCoroutine(moveCoro);
            moveCoro = null;
        }
    }

    protected override void internalStartMakingMove(in Board board)
    {
        Utility.MyDebugAssert(isMoving == false, "internalStartMakingMove() called when already moving.");

        isMoving = true;

        // the actual work is done in the coroutine per frame.
        moveCoro = this.StartCoroutine(humanMoveCoro(board));
    }

    /********************************** HELPERS ************************************/

    /// <summary>
    /// Whenever this coroutine is resumed (per frame), 
    /// it checks the user input and updates this.move.
    /// When it sees an end move, it completes making the move.
    /// 
    /// </summary>
    /// <param name="board">reference to the current board</param>
    private IEnumerator humanMoveCoro(Board board)
    {
        // Every frame, perfrom checkUserInputPerFrame()
        // If it does not return true, i.e., the end move is not performed,
        // then it suspends and wait for execution in the next frame.
        while (!checkUserInputPerFrame(board))
        {
            yield return null;
        }

        // Otherwise, it completes making the move by setting isMoving = false.
        isMoving = false;
    }

    /// <summary>
    /// Is called every frame during the human's making a move.
    /// 
    /// Checks the user input and updates this.move.
    /// When it sees an end move, returns true.
    /// 
    /// When it sees the action timer is fired, then 
    ///     1. it sets this.move to forfeited.
    ///     2. returns true.
    /// </summary>
    /// <param name="board">reference to the current board</param>
    /// <returns>true iff an end move has been performed by the user.</returns>
    private bool checkUserInputPerFrame(Board board)
    {
        throw new System.NotImplementedException();
    }
}