

using UnityEngine.Rendering;

/// <summary>
/// Represents a chess player.
/// 
/// Because making a chess move needs time,
///     (for a human, he needs to perform a series of actions;
///     for an AI, it needs to calculate the move)
/// A player makes move asynchronously, and is controlled by three methods.
/// 
/// Call startMakingMove(), 
/// wait for hasFinishedMakingMove() to become true, 
/// and call getMove().
/// </summary>
public interface IChessPlayer
{
    /********************************** METHODS ************************************/

    /// <summary>
    /// Assume that it's this player's turn.
    /// Call this method to signal the player to start making a move.
    /// </summary>
    /// <param name="board">
    /// Reference to the current board. 
    /// An impl may make a copy of the board.
    /// </param>
    /// 
    /// <exception cref="System.InvalidOperationException">
    /// if a move is already being made
    /// </exception>
    public void startMakingMove(in Board board);

    /// <returns>
    /// true iff a move has been made. 
    /// Equivalently, iff there is currently no move being made.
    /// </returns>
    public bool hasFinishedMakingMove();

    /// <summary>
    /// Called when the game is restarted or goes back to the main ui.
    /// </summary>
    public void reset();

    /// <returns>
    /// The move most recently made.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// 1. if makeMove() has not been called.
    /// 2. if a move is being made.
    /// </exception>
    public ChessMove getMove();

}
