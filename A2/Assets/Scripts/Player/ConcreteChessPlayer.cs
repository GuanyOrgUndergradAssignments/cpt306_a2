using UnityEngine;

/// <summary>
/// A concrete chess player implementation.
/// 
/// Inherit from MonoBehaviour so that we can use Unity's functions
/// for coroutine and threads.
/// 
/// Invariant:
///     1. hasMovedAtAll is as its name says
///     2. side is either PLAYER1 or PLAYER2
///     3. move equals to the move made if hasFinishedMakingMove() = true.
/// </summary>
public abstract class ConcreteChessPlayer : MonoBehaviour, IChessPlayer
{
    /********************************** FIELDS ************************************/
    // which side the player is on
    private readonly Board.BoardPositionState side;

    private bool hasMovedAtAll = false;
    // derived classes should fill in this field during making a move.
    protected ChessMove move;

    /********************************** CTOR ************************************/

    /// <param name="side">PLAYER1 or PLAYER2</param>
    /// <exception cref="System.ArgumentException">if side is not PLAYER1 or PLAYER2</exception>
    public ConcreteChessPlayer(Board.BoardPositionState side)
    {
        if (side != Board.BoardPositionState.PLAYER1 && side != Board.BoardPositionState.PLAYER2)
        {
            throw new System.ArgumentException("side value is not correct.");
        }

        this.side = side;
        }

    /********************************** OBSERVERS ************************************/

    public Board.BoardPositionState getSide() { return side; }

    public bool isPlayer1() { return side == Board.BoardPositionState.PLAYER1; }
    public bool isPlayer2() { return side == Board.BoardPositionState.PLAYER2; }

    /********************************** FROM IChessPlayer ************************************/
    public void startMakingMove(in Board board)
    {
        if (!hasFinishedMakingMove())
        {
            throw new System.InvalidOperationException("already moving.");
        }

        hasMovedAtAll = true;

        internalStartMakingMove(board);
    }

    public abstract bool hasFinishedMakingMove();

    /// <summary>
    /// Override this to implement the actual logic of start making a move.
    /// </summary>
    protected abstract void internalStartMakingMove(in Board board);

    public ChessMove getMove()
    {
        if (!hasMovedAtAll)
        {
            throw new System.InvalidOperationException("has not moved at all.");
        }

        if (!hasFinishedMakingMove())
        {
            throw new System.InvalidOperationException("Is moving.");
        }

        return move;
    }

    public abstract void reset();
}