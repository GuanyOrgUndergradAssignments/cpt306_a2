using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    /// <summary>
    /// check if the move is a clone move
    /// </summary>
    /// <param name="move"></param>
    /// <returns>
    /// return true when:
    ///     1. src and des are not null
    ///     2. src and des are both in board
    ///     3. src and des are not in the same pos
    ///     5. Manhattan distance between src and dst in the range of 1</returns>
    public static bool isClone(ChessMove move)
    {
        Vector2Int src = move.getSrc();
        Vector2Int dst = move.getDst();
        
        if(src == null || dst == null)
        {
            return false;
        }

        if(isPosInBoard(src) || !isPosInBoard(dst))
        {
            return false;
        }

        if(src.Equals(dst))
        {
            return false;
        }
        
        return Math.Abs(src.x - dst.x) <= 1 && Math.Abs(src.y - dst.y) <= 1;
    }
    
    /// <summary>
    /// check if the move is a jump move
    /// </summary>
    /// <param name="move"></param>
    /// <returns>
    /// return true when:
    ///     1. src and des are not null
    ///     2. src and des are both in board
    ///     3. src and des are not in the same pos
    ///     4. not clone move
    ///     5. Manhattan distance between src and dst in the range of 2</returns>
    public static bool isJump(ChessMove move)
    {
        Vector2Int src = move.getSrc();
        Vector2Int dst = move.getDst();
        
        if(src == null || dst == null)
        {
            return false;
        }

        if(isPosInBoard(src) || !isPosInBoard(dst))
        {
            return false;
        }

        if(src.Equals(dst))
        {
            return false;
        }

        if (!isClone(move))
        {
            return false;
        }
        return Math.Abs(src.x - dst.x) <= 2 && Math.Abs(src.y - dst.y) <= 2;
    }

    /********************************** FIELDS ************************************/

    // TODO: Defined By Zhen Ma
    private BoardPositionState[,] board;

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
    public Board
    (
        UnityEngine.Vector2Int player1StartPos, 
        UnityEngine.Vector2Int player2StartPos
    )
    {
        // initialise the board
        this.board = new BoardPositionState[BOARD_LENGTH, BOARD_LENGTH];
        for (int i = 0; i < BOARD_LENGTH; i++)
        {
            for (int j = 0; j < BOARD_LENGTH; j++)
            {
                if (i == player1StartPos.x && j == player1StartPos.y)
                {
                    this.board[i, j] = BoardPositionState.PLAYER1;
                }
                else if(i == player2StartPos.x && j == player2StartPos.y)
                {
                    this.board[i, j] = BoardPositionState.PLAYER2;
                }
                else
                {
                    this.board[i, j] = BoardPositionState.EMPTY;
                }
            }
        }
        
        // TODO: Defined By Zhen Ma (I have only initialised the board cuz I need it. Other functions have not implemented)
        
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
    /// <returns>the current state
    /// In my(kemu) realization, the state is: (--> need to be checked by Zhen Ma)
    /// BoardState.PLAYER2_WON if player2 has won (1. player1 has no pawns left; OR 2. both players cannot move and player2 has more pawns);
    /// BoardState.PLAYER1_WON if player1 has won (1. player2 has no pawns left; OR 2. both players cannot move and player1 has more pawns)
    /// BoardState.DRAW if the game is draw (both player cannot move and have the same number of pawns).
    /// BoardState.PLAYING if the game is not end (no situation above happens);
    /// </returns>
    public BoardState getBoardState()
    {
        Vector3Int result = getBoardStatistics();
        if(result.y == 0)
        {
            return BoardState.PLAYER2_WON;
        }else if (result.z == 0)
        {
            return BoardState.PLAYER1_WON;
        }else if(!couldMove(BoardPositionState.PLAYER1) && !couldMove(BoardPositionState.PLAYER2))
        {
            if (result.y > result.z)
            {
                return BoardState.PLAYER1_WON;
            }
            else if (result.y < result.z)
            {
                return BoardState.PLAYER2_WON;
            }
            else
            {
                return BoardState.DRAW;
            }
        }
        else
        {
            return BoardState.PLAYING;
        }
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
    /// <param name="move">the chess move</param>
    /// <returns>what the board would become if the move was made.</returns>
    /// 
    /// <exception cref="System.ArgumentException">
    /// 1. if the move is illegal or not possible.
    /// </exception>
    public Board contemplateMove
    (
        ChessMove move
    )
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// In this function, we ensure this move is legal in the board
    /// a move is legal when the following requirements are met:
    ///     1. the player is not empty
    ///     2. Manhattan distance between src and dst in the range of 2
    ///         -- move is forfeit
    ///             1. one player could move (--> should be discussed somehow, I am not sure)
    ///         -- move is not forfeit
    ///             1. des is in the range of board and not empty
    ///             2. current player is at src
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public bool checkMove(ChessMove move)
    {
        // Determine the current player
        BoardPositionState currentPlayer = move.getPlayer();
        
        // check if the player is empty
        if (currentPlayer == BoardPositionState.EMPTY)
        {
            return false;
        }
        
        // check if the dst is in the range of 5*5 box taking the src as the center
        Vector2Int src = move.getSrc();
        Vector2Int dst = move.getDst();
        if (!(Math.Abs(src.x - dst.x) <= 2 && Math.Abs(src.y - dst.y) <= 2))
        {
            return false;
        }

        // Determine the next player
        BoardPositionState nextPlayer = (currentPlayer == BoardPositionState.PLAYER1 ? 
            BoardPositionState.PLAYER2 : BoardPositionState.PLAYER1);
            
        // Check if the move is forfeited
        if (move.isForfeited())
        {
            return (couldMove(currentPlayer) || couldMove(nextPlayer));
        }
        else
        {
            if(!isPosInBoard(move.getDst())
               || getBoardPosState(move.getDst()) != BoardPositionState.EMPTY)
            {
                return false;
            }
            else
            {
                if (getBoardPosState(move.getSrc()) != currentPlayer)
                {
                    return false;
                }

                return true;
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="who"></param>
    /// <returns> Return true iff player WHO could move, ignoring whether it is
    /// that player's move and whether the getAtaxxGame is over.</returns>
    private bool couldMove(BoardPositionState who)
    {
        for (int i = 0; i < BOARD_LENGTH; i++)
        {
            for (int j = 0; j < BOARD_LENGTH; j++)
            {
                if (getBoardPosState(new Vector2Int(i, j)) == who)
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (m !=0 && n != 0)
                            {
                                Vector2Int temp = new Vector2Int(i + m, j + n);
                                if(temp.x >= 0 
                                   && temp.x < BOARD_LENGTH 
                                   && temp.y >= 0 
                                   && temp.y < BOARD_LENGTH
                                   && getBoardPosState(temp) == BoardPositionState.EMPTY)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /********************************** MUTATORS ************************************/

    // REMINDER: the statement enclosed by "()" is added by kemu
    /// <summary>
    /// Make a move on the board.
    /// </summary>
    /// <returns>
    /// A list of all positions where the opponent's pawns
    /// have been turned into the pawns of the player who made the move.
    /// 
    /// The game may use the list to know which GameObjects to create/destroy,
    /// and which effects to play.
    /// </returns>
    /// 
    /// <exception cref="System.ArgumentException">
    /// 1. if the move is illegal or not possible.
    /// (2. if the game is end.)
    /// </exception>
    public List<UnityEngine.Vector2Int> makeMove
    (
        ChessMove move
    )
    {
        // throw an exception if the move is illegal or the game is end
        if(!checkMove(move) 
           || !isClone(move)
           || !isJump(move)
           || getBoardState() != BoardState.PLAYING)
        {
            throw new System.ArgumentException("The move is illegal or the game is end.");
        }

        // return an empty list if the move is forfeited
        if (move.isForfeited())
        {
            return new List<Vector2Int>();
        }

        List<UnityEngine.Vector2Int> result = new List<Vector2Int>();
        BoardPositionState opponent = (move.getPlayer() == BoardPositionState.PLAYER1 ? 
            BoardPositionState.PLAYER2 : BoardPositionState.PLAYER1);

        // if the move is a jump move, remove the src
        if (isJump(move))
        {
            board[move.getSrc().x, move.getSrc().y] = BoardPositionState.EMPTY;
        }
        
        // convert dst to player's pawn, and all opponent's pawns within the Manhattan distance of 1 to player's pawn
        board[move.getDst().x, move.getDst().y] = move.getPlayer();
        for (int i = move.getDst().x - 1; i <= move.getDst().x + 1; i++)
        {
            for (int j = move.getDst().y - 1; j <= move.getDst().y + 1; j++)
            {
                if (getBoardPosState(new Vector2Int(i, j)) == opponent)
                {
                    board[i, j] = move.getPlayer();
                    result.Add(new Vector2Int(i, j));
                }
            }
        }
        return result;
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
