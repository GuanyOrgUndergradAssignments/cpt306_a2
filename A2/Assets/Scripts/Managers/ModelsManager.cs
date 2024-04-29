using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the board and pawn models.
/// 
/// Has to be a MonoBehaviour to be able to have audio attached to it.
/// </summary>
public class ModelsManager: MonoBehaviour
{
    /*********************************** FIELDS ***********************************/
    // given in then constructor
    public GameObject boardPrefab;
    // for player 1
    public GameObject pawn1Prefab;
    // for player 2
    public GameObject pawn2Prefab;

    // TODO: some fields to store all pawns and the board.

    // TODO: some visual and audio effects

    /*********************************** CTOR ***********************************/
    public ModelsManager()
    {
    }

    /*********************************** MONO ***********************************/
    /// <summary>
    /// Check the prefabs
    /// </summary>
    public void Awake()
    {
        Utility.MyDebugAssert(boardPrefab != null, "check prefabs in editor.");
        Utility.MyDebugAssert(pawn1Prefab != null, "check prefabs in editor.");
        Utility.MyDebugAssert(pawn2Prefab != null, "check prefabs in editor.");

        throw new System.NotImplementedException("Check the effect prefabs.");
    }

    /// <summary>
    /// Destroys everything.
    /// </summary>
    public void OnDestroy()
    {
        // first, destroy the pawns
        clear();

        // then, destroy the board
        throw new System.NotImplementedException();

        // lastly, clear the effect prefabs.
        throw new System.NotImplementedException();
    }

    /*********************************** METHODS ***********************************/
    /// <summary>
    /// Spawn the board and the two initial pawns.
    /// </summary>
    /// <param name="player1Pawn"></param>
    /// <param name="player2Pawn"></param>
    public void onGameStart(Vector2Int player1Pawn, Vector2Int player2Pawn)
    {
        playGameStartEffects(player1Pawn, player2Pawn);

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// After the move is made, sync its changes to the models.
    /// 
    /// 1. play the move effect
    /// 2. destroy killed pawns
    /// 3. spawn new pawns
    /// </summary>
    /// <param name="move">the move</param>
    /// <param name="replacedPawns">returned by Board.makeMove()</param>
    /// <param name="whoseMove">who made the move</param>
    public void onMoveMade(ChessMove move, List<Vector2Int> replacedPawns, Board.BoardPositionState whoseMove)
    {
        playMoveEffects(move, replacedPawns, whoseMove);

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Destroys all pawns on the board.
    /// </summary>
    public void clear()
    {
        throw new System.NotImplementedException();
    }

    /*********************************** PRIVATE HELPERS ***********************************/
    /// <summary>
    /// Play the visual and audio effects on game start.
    /// </summary>
    private void playGameStartEffects(Vector2Int player1Pawn, Vector2Int player2Pawn)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Play the visual and audio effects on move made
    /// Can only be called by onMoveMade()
    /// </summary>
    /// <param name="move">the move, passed in by onMoveMade()</param>
    /// <param name="replacedPawns">returned by Board.makeMove(), passed in by onMoveMade()</param>
    /// <param name="whoseMove">who made the move, passed in by onMoveMade()</param>
    private void playMoveEffects(ChessMove move, List<Vector2Int> replacedPawns, Board.BoardPositionState whoseMove)
    {
        throw new System.NotImplementedException();
    }
}