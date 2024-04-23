using System;
using System.Collections.Generic;

/// <summary>
/// A Board stores the state of the board and the pawns
/// and defines methods of moving them.
/// </summary>
/// <remarks>
/// Note that a Board has absolutely no GameObjects, meshes, and effects.
/// It is just a matrix of data indicating where the pawns are.
/// 
/// For use with AI and the game, the Board class has two versions of making a chess move.
/// One version, contemplateMove(),
///     does not modify its data, but returns a new Board as the result of the move.
/// Another version, makeMove(),
///     applies the move to its data, and returns a list of the pos of the pawns turned by the move.
/// </remarks>
public class Board
{
    /********************************** STATIC MEMBERS ************************************/

    // A Board is a LENGTH * LENGTH matrix
    // (A const definition cannot be marked as static, but is static anyway).
    public const uint BOARD_LENGTH = 8;
    public const uint BOARD_NUM_POSITIONS = BOARD_LENGTH * BOARD_LENGTH;

    /// <param name="pos">(0,0) to (length-1, length-1)</param>
    /// <returns>true iff pos is within the board</returns>
    public static bool isPosInBoard(UnityEngine.Vector2 pos)
    {
        return
            pos.x >= 0 && pos.x < BOARD_LENGTH &&
            pos.y >= 0 && pos.y < BOARD_LENGTH;
    }

    /// <summary>
    /// State of the whole board
    /// </summary>
    public enum BoardState
    {
        // The game has not ended yet.
        PLAYING,
        // Player1 has won
        PLAYER1_WON,
        // Player2 has won
        PLAYER2_WON,
        // Draw
        DRAW
    }

    /// <summary>
    /// State of a position on the board
    /// </summary>
    public enum BoardPositionState
    {
        // The position has no pawn.
        EMPTY,
        // A player1's pawn is on this position.
        PLAYER1,
        // A player2's pawn is on this position.
        PLAYER2
    }

    /********************************** FIELDS ************************************/

    // TODO: Defined By Zhen Ma

    /********************************** CTOR ************************************/

    /// <summary>
    /// Initialises the board to the game start state.
    /// </summary>
    /// <param name="player1StartPos">where player1's first pawn is.</param>
    /// <param name="player2StartPos">where player2's first pawn is.</param>
    /// 
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if any position is out of range
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// if the two positions are the same.
    /// </exception>
    public Board(UnityEngine.Vector2Int player1StartPos, UnityEngine.Vector2Int player2StartPos)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Copies the state of another board to create a new board.
    /// </summary>
    /// <param name="other"></param>
    public Board(Board other)
    {
        throw new System.NotImplementedException();
    }

    /********************************** OBSERVERS ************************************/

    /// <summary>
    /// Calculates the state of the board.
    /// </summary>
    /// <returns>the current state</returns>
    public BoardState getBoardState()
    {
        throw new System.NotImplementedException();
    }

    /// <param name="position"></param>
    /// <returns>The state of the position</returns>
    /// 
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if position is out of board
    /// </exception>
    public BoardPositionState getBoardPosState(UnityEngine.Vector2Int position)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// One might want to get the number of a player's pawns sometimes.
    /// However, either it is to get any player's pawn or number of empty places, 
    /// we have to iterate through the whole board anyway.
    /// 
    /// Therefore, I put all of them in one method,
    /// so that we only have to iterate through the board once.
    /// 
    /// Better yet, we could store the numbers in the class,
    /// and only change them when a move is made.
    /// </summary>
    /// <returns>
    /// (num of empty pos, num of player1's pawns, num of player2's pawns)
    /// </returns>
    public UnityEngine.Vector3Int getBoardStatistics()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Contemplate what would happen after a player makes a move.
    /// This method does not modify the current board.
    /// Could be called by an AI to evaluate possible outcomes.
    /// </summary>
    /// <param name="src">source position of the move</param>
    /// <param name="dst">destination position of the move</param>
    /// <param name="player">
    /// which player to make the move. 
    /// Can only be either PLAYER1 or PLAYER2
    /// </param>
    /// <returns>what the board would become if the move was made.</returns>
    /// 
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if any position is out of board
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// 1. if the move is illegal or not possible.
    /// 2. if player is not one of PLAYER1 and PLAYER2.
    /// </exception>
    public Board contemplateMove
    (
        UnityEngine.Vector2Int src,
        UnityEngine.Vector2Int dst,
        BoardPositionState player
    )
    {
        throw new System.NotImplementedException();
    }

    /********************************** MUTATORS ************************************/

    /// <summary>
    /// Make a move on the board.
    /// </summary>
    /// <param name="src">source position of the move</param>
    /// <param name="dst">destination position of the move</param>
    /// <param name="player">
    /// which player to make the move. 
    /// Can only be either PLAYER1 or PLAYER2
    /// </param>
    /// <returns>
    /// A list of all posistions where the opponent's pawns
    /// have been turned into the pawns of the player who made the move.
    /// 
    /// The game may use the list to know which GameObjects to create/destroy,
    /// and which effects to play.
    /// </returns>
    /// 
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if any position is out of board
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// 1. if the move is illegal or not possible.
    /// 2. if player is not one of PLAYER1 and PLAYER2.
    /// </exception>
    public List<UnityEngine.Vector2Int> makeMove
    (
        UnityEngine.Vector2Int src,
        UnityEngine.Vector2Int dst,
        BoardPositionState player
    )
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Resets the board to the state as if a game has just started.
    /// </summary>
    /// <param name="player1StartPos">where player1's first pawn is.</param>
    /// <param name="player2StartPos">where player2's first pawn is.</param>
    /// 
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// if any position is out of range
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// if the two positions are the same.
    /// </exception>
    public void reset(UnityEngine.Vector2Int player1StartPos, UnityEngine.Vector2Int player2StartPos)
    {
        throw new System.NotImplementedException();
    }

}
