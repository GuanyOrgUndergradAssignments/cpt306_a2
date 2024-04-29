using System;
using System.Threading;

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
        throw new System.NotImplementedException();
    }
}