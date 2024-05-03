using System;
using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// Represents an AI chess player.
/// </summary>
public class AIChessPlayer : ConcreteChessPlayer
{
    /********************************** FIELDS ************************************/
    private Thread aiThread = null;
    // Used to terminate the aiThread
    private IntPtr aiThreadHandle = default(IntPtr);
    private Board chessBoardRef;
    
    
    // determine the AI difficulty, namely the step depth AI will contemplate
    private int AIDifficulty;
    
    // the final chess move ai will take
    private ChessMove lastFoundMove;

    // TODO: decided by Kemu Xu
    private readonly uint difficulty;
    private readonly ChessMove passMove;

    private enum AIDifficultyState
    {
        EASY = 1,
        NORMAL = 2,
        HARD = 3
    }

    /********************************** CTOR ************************************/
    public AIChessPlayer(Board.BoardPositionState side, uint difficulty) : base(side)
    {
        this.difficulty = difficulty;
    }

    /********************************** FROM ConcreteChessPlayer ************************************/

    public override bool hasFinishedMakingMove()
    {
        // if there is an ai thread and it's already finished.
        return aiThread != null && !aiThread.IsAlive;
    }

    /// <summary>
    /// Kill the aiThread
    /// </summary>
    public override void reset()
    {
        if(aiThread != null && aiThread.IsAlive)
        {
            // Block until the thread has set its handle value.
            while (aiThreadHandle == default(IntPtr)) { }

            // Why deprecate Thread.Abort()?
            Utility.TerminateThread(aiThreadHandle, 0);

            aiThread = null;
            aiThreadHandle = default(IntPtr);
        }
    }

    protected override void internalStartMakingMove(in Board board)
    {
        chessBoardRef = board;

        aiThread = new Thread(new ThreadStart(this.threadTask));
        aiThread.IsBackground = true;
        aiThread.Start();
    }

    /********************************** HELPERS ************************************/
    private void threadTask()
    {
        aiThreadHandle = Utility.GetCurrentThread();
        this.move = aiTask();
    }

    /// <summary>
    /// According to 
    ///     1. this.chessBoardRef,
    ///     2. this.difficulty
    /// calcualte the next move.
    /// </summary>
    /// <returns>the move made</returns>
    private ChessMove aiTask()
    {
        AIDifficulty = (int)(AIDifficultyState)difficulty;
        ChessMove aiMove = findMove();
        
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// find the best move for the AI player by the miniMax algorithm
    /// </summary>
    /// <returns>the best move</returns>
    private ChessMove findMove()
    {
        Board contemplateBoard = new Board(chessBoardRef);
        // default chess move to help check the existence of valid move,
        // default chess move is to stay the last move to the destination position
        lastFoundMove = new ChessMove(getSide(), getMove().getDst(), getMove().getDst());
        
        // Minimax Search
        miniMax(contemplateBoard, AIDifficulty, true, 1, int.MinValue, int.MaxValue);

        return lastFoundMove;
    }

    /// <summary>
    /// Use Minimax algorithm with alpha-beta pruning
    /// to RECURSIVELY find a move from position BOARD and return its
    /// alpha or beta value, recording the move found in lastFoundMove iff saveMove.
    /// For Max Operation, if SENSE == 1,
    /// The move should have maximal value or have value >= BETA -> pruning,
    /// For Min Operation, if SENSE == -1,
    /// The move should have minimal value or have value <= ALPHA -> pruning.
    /// Searches up to DEPTH levels. Searching at level 0 simply returns a static estimate
    /// of the board value and does not set lastFoundMove. If the game is over
    /// on BOARD, does not set lastFoundMove either.
    /// </summary>
    /// <param name="board">current board to find a move.</param>
    /// <param name="depth">denotes search depth from MAX_DEPTH to 0, minus 1 reach call.</param>
    /// <param name="saveMove">indicates only the first recursive call should save the found move.</param>
    /// <param name="sense">uses 1 and -1 to indicate Min (-1) or Max (1) operations.</param>
    /// <param name="alpha">denotes the lower bound of the best known choice when searching to the current node.</param>
    /// <param name="beta">denotes the upper bound of the worst ending when searching down from this node.</param>
    /// <returns>the alpha or beta value depending on min or max operation.</returns>
    private int miniMax(Board board, int depth, bool saveMove, int sense, int alpha, int beta)
    {
        // initialize the return value
        int alphaOrBeta = 0;
        // get the list of all possible ChessMove including forfeit move
        ArrayList listOfMoves = possibleMoves(board, getSide());
        bool passLegal;
        try
        {
            passLegal = board.checkMove(passMove);
        }
        catch (Exception e)
        {
            passLegal = false;
        }

        if (passLegal)
        {
            listOfMoves.Add(passMove);
        }

        // make the sequence of the move list random
        listOfMoves = new ArrayList(makeSequenceRandom(listOfMoves));

        // max operation
        if (sense == 1)
        {
            foreach (ChessMove move in listOfMoves)
            {
                Board copy = new Board(board);
                copy.makeMove(move);
                // get the worst scores selected from its child moves
                int response = miniMax(copy, depth - 1, false, -1, alpha, beta);
                // select the best score from the worst scores
                if (response > alpha)
                {
                    alpha = response;
                    if (saveMove)
                    {
                        lastFoundMove = move;
                    }
                }
                // pruning
                if (alpha >= beta)
                {
                    break;
                }
            }

            alphaOrBeta = alpha;
        }
        // min operation
        else if (sense == -1)
        {
            foreach (ChessMove move in listOfMoves)
            {
                Board copy = new Board(board);
                copy.makeMove(move);
                // get the best scores selected from its child moves
                int response = miniMax(copy, depth - 1, false, 1, alpha, beta);
                // select the worst score from the best scores
                if (response < beta)
                {
                    beta = response;
                }
                // pruning
                if(alpha >= beta)
                {
                    break;
                }
            }

            alphaOrBeta = beta;
        }

        return alphaOrBeta;
    }

    /// <summary>
    /// make the sequence of the move list in a random way instead of the search way
    /// </summary>
    /// <param name="listOfMoves"></param>
    /// <returns>the list of moves in a random sequence</returns>
    private ArrayList makeSequenceRandom(ArrayList listOfMoves)
    {
        for (int i = 0; i < listOfMoves.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, listOfMoves.Count);
            ChessMove temp = (ChessMove)listOfMoves[i];
            listOfMoves[i] = listOfMoves[randomIndex];
            listOfMoves[randomIndex] = temp;
        }

        return listOfMoves;
    }

    /// <summary>
    /// get all possible moves for the player
    /// In this function, all positions owned by player are iterated for possible moves.
    /// And all possible moves for each position are checked by the assistPossibleMoves function.
    /// </summary>
    /// <param name="board"> the current board</param>
    /// <param name="player">the player who will make the chess move</param>
    /// <returns> all possible moves for the player</returns>
    private ArrayList possibleMoves(Board board, Board.BoardPositionState player)
    {
        ArrayList possibleMoves = new ArrayList();

        for (int i = 0; i < Board.BOARD_LENGTH; i++)
        {
            for (int j = 0; j < Board.BOARD_LENGTH; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if (board.getBoardPosState(pos) == player)
                {
                    ArrayList addMoves = assistPossibleMoves(board, player, pos);
                    possibleMoves.AddRange(addMoves);
                }
            }
        }

        return possibleMoves;
    }

    /// <summary>
    /// get all legal moves in the board for the player at the position 'pos' to make
    /// </summary>
    /// <param name="board">the current board</param>
    /// <param name="player">the player who will make the chess move</param>
    /// <param name="pos">the original position where the player will move from</param>
    /// <returns>all legal moves in the board for the player at the position 'pos' to make</returns>
    private ArrayList assistPossibleMoves(Board board, Board.BoardPositionState player, Vector2Int pos)
    {
        ArrayList assistPossibleMoves = new ArrayList();
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if(i != 0 && j != 0)
                {
                    Vector2Int possibleDst = new Vector2Int(pos.x + i, pos.y + j);
                    ChessMove possibleMove = new ChessMove(player, pos, possibleDst);
                    if (board.checkMove(possibleMove))
                    {
                        assistPossibleMoves.Add(possibleMove);
                    }
                }
            }
        }

        return assistPossibleMoves;
    }

}