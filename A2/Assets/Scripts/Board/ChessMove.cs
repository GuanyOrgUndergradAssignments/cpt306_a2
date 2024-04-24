using UnityEngine;

/// <summary>
/// Represents a chess move made by a player.
/// Immutable.
/// 
/// Invariants:
///     1. whichPlayer is either PLAYER1 or PLAYER2
///     2. src and dst falls within the board's range.
/// </summary>
public readonly struct ChessMove
{
    /********************************** FIELDS ************************************/

    private readonly Board.BoardPositionState whichPlayer;
    
    // If src = dst, then the move is treated as forfeited.
    private readonly UnityEngine.Vector2Int src;
    private readonly UnityEngine.Vector2Int dst;

    /********************************** CTOR ************************************/

    /// <summary>
    /// The only constructor.
    /// </summary>
    /// <param name="src">source position of the move</param>
    /// <param name="dst">destination position of the move</param>
    /// <param name="player">
    /// which player to make the move. 
    /// Can only be either PLAYER1 or PLAYER2
    /// </param>
    /// 
    /// <exception cref="System.ArgumentException">
    /// if player is not PLAYER1 or PLAYER2.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if src or dst is out of range.
    /// </exception>
    public ChessMove
    (
        Board.BoardPositionState whichPlayer,
        UnityEngine.Vector2Int src,
        UnityEngine.Vector2Int dst
    )
    {
        if(whichPlayer != Board.BoardPositionState.PLAYER1 || whichPlayer != Board.BoardPositionState.PLAYER2)
        {
            throw new System.ArgumentException("player value is not correct.");
        }

        if(!Board.isPosInBoard(src) || !Board.isPosInBoard(dst))
        {
            throw new System.ArgumentOutOfRangeException("src or dst is out of range.");
        }

        this.whichPlayer = whichPlayer;
        this.src = src;
        this.dst = dst;
    }

    /********************************** OBSERVERS ************************************/

    /// <returns>true iff this move is forfeited, i.e. src == dst</returns>
    public bool isForfeited() { return src == dst; }

    public Board.BoardPositionState getPlayer() { return whichPlayer; }
    public Vector2Int getSrc() { return src; }
    public Vector2Int getDst() { return dst; }
}