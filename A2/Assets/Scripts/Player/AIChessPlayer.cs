using System.Threading;

/// <summary>
/// Represents an AI chess player.
/// </summary>
public class AIChessPlayer : ConcreteChessPlayer
{
    /********************************** FIELDS ************************************/
    private Thread aiThread = null;
    private Board chessBoardRef;

    // TODO: decided by Kemu Xu
    private readonly uint difficulty;

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
        throw new System.NotImplementedException();
    }
}