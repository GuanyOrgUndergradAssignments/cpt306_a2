using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

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
    // visual effects
    public GameObject blueEffect;
    public GameObject redEffect;
    // audio effects
    public AudioClip jump, clone, colorChange;
    private AudioSource audioSource;
    private List<GameObject> pawns = new List<GameObject>();
    private GameObject board;

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

        Utility.MyDebugAssert(blueEffect != null, "check prefabs in editor.");
        Utility.MyDebugAssert(redEffect != null, "check prefabs in editor.");
    }

    /// <summary>
    /// Destroys everything.
    /// </summary>
    public void OnDestroy()
    {
        // first, destroy the pawns
        clear();

        // then, destroy the board
        Destroy(board);
    }

    /*********************************** OBSERVERS ***********************************/

    // These returns properties (e.g. dimensions, position, rotation)
    // of the models

    // Most will be used by the Camera manager.

    /// <returns>
    /// The position of the center of the top surface plane of the board.
    /// The value should not change at all.
    /// </returns>
    public Vector3 getBoardSurfaceCenter()
    {
        return board.transform.position;
    }

    /// <returns>
    /// NOT USED FOR NOW.
    /// (x,z), where 
    /// x is the size of the board along the x axis,
    /// z is the size of the board along the z axis
    /// </returns>
    //public Vector2 getBoardSize2D()
    //{
    //    throw new System.NotImplementedException();
    //}

    /*********************************** MUTATORS ***********************************/

    /// <summary>
    /// Spawn the board and the two initial pawns.
    /// </summary>
    /// <param name="player1Pawn"></param>
    /// <param name="player2Pawn"></param>
    public void onGameStart(Vector2Int player1Pawn, Vector2Int player2Pawn)
    {
        playGameStartEffects(player1Pawn, player2Pawn);
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
    }

    /// <summary>
    /// Destroys all pawns on the board.
    /// </summary>
    public void clear()
    {
        foreach (GameObject pawn in pawns)
        {
            Destroy(pawn);
            pawns.Remove(pawn);
        }
    }

    /*********************************** PRIVATE HELPERS ***********************************/
    /// <summary>
    /// Play the visual and audio effects on game start.
    /// </summary>
    private void playGameStartEffects(Vector2Int player1Pawn, Vector2Int player2Pawn)
    {
        // Instantiate the board
        board = Instantiate(boardPrefab, Vector3.zero, Quaternion.identity);

        // Calculate world ositions for pawns
        Vector3 pawn1Position = BoardPositionToWorld(player1Pawn);
        Vector3 pawn2Position = BoardPositionToWorld(player2Pawn);

        // Instantiate pawns
        GameObject pawn1 = Instantiate(pawn1Prefab, pawn1Position, Quaternion.identity);
        GameObject pawn2 = Instantiate(pawn2Prefab, pawn2Position, Quaternion.identity);

        // Add pawns to the list
        pawns.Add(pawn1);
        pawns.Add(pawn2);
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
        Vector3 srcPosition = BoardPositionToWorld(move.getSrc());
        Vector3 dstPosition = BoardPositionToWorld(move.getDst());

        // check move type
        if (Board.isJump(move))
        {
            foreach (GameObject pawn in pawns)
            {
                if (pawn.transform.position == srcPosition)
                {
                    StartCoroutine(JumpEffect(pawn, srcPosition, dstPosition, () =>
                    {
                        // Move completed, handle replaced pawns
                        HandleReplacedPawns(replacedPawns, whoseMove);
                    }));
                }
            }            
        }
        if (Board.isClone(move))
        {
            GameObject clonedPawn = InstantiatePawnAtPosition(srcPosition, whoseMove);
            pawns.Add(clonedPawn);
            StartCoroutine(CloneEffect(clonedPawn, srcPosition, dstPosition, () =>
            {
                // Move completed, handle replaced pawns
                HandleReplacedPawns(replacedPawns, whoseMove);
            }));       
        }
    }

    private void HandleReplacedPawns(List<Vector2Int> replacedPawns, Board.BoardPositionState whosemove)
    {
        List<GameObject> effects = new List<GameObject>();

        // Destroy killed pawns
        foreach (Vector2Int replacedPawnPos in replacedPawns)
        {
            Vector3 pawnPosition = BoardPositionToWorld(replacedPawnPos);

            // Instantiate effect at pawn position
            GameObject effect = InstantiateEffectAtPosition(pawnPosition, whosemove);

            audioSource = effect.GetComponent<AudioSource>();
            audioSource.clip = colorChange;
            audioSource.Play();

            effects.Add(effect);
            DestroyPawnAtPosition(pawnPosition);
            Destroy(effect, 1.0f);
        }

        // Spawn new pawns
        foreach (Vector2Int replacedPawnPos in replacedPawns)
        {
            Vector3 pawnPosition = BoardPositionToWorld(replacedPawnPos);
            GameObject newPawn = InstantiatePawnAtPosition(pawnPosition, whosemove);
            pawns.Add(newPawn);
        }

    }

    private Vector3 BoardPositionToWorld(Vector2Int boardPosition)
    {
        float cellSize = 0.18f;
        float boardHeight = 0.11f;

        float x = (boardPosition.x - 3.5f) * cellSize;
        float y = boardHeight;
        float z = (boardPosition.y - 3.5f) * cellSize;
       
        return new Vector3(x, y, z);
    }

    // Helper method to instantiate a pawn at a given position based on the current player
    private GameObject InstantiatePawnAtPosition(Vector3 position, Board.BoardPositionState player)
    {
        GameObject pawnPrefab = (player == Board.BoardPositionState.PLAYER1) ? pawn1Prefab : pawn2Prefab;
        return Instantiate(pawnPrefab, position, Quaternion.identity);
    }

    // Helper method to instantiate an effect at a given position based on the current player
    private GameObject InstantiateEffectAtPosition(Vector3 position, Board.BoardPositionState player)
    {
        GameObject effectPrefab = (player == Board.BoardPositionState.PLAYER1) ? blueEffect : redEffect;
        return Instantiate(effectPrefab, position, Quaternion.identity);
    }

    // Helper method to destroy a pawn at a given position
    private void DestroyPawnAtPosition(Vector3 position)
    {
        foreach (GameObject pawn in pawns)
        {
            if (pawn.transform.position == position)
            {
                Destroy(pawn);
                pawns.Remove(pawn);
                break;
            }
        }
    }

    // Helper method for jump effect
    private IEnumerator JumpEffect(GameObject gameObject, Vector3 srcPosition, Vector3 dstPosition, Action onComplete)
    {
        float jumpHeight = 0.3f;
        float jumpDuration = 0.5f;
        float jumpTimer = 0f;

        Vector3 startPos = gameObject.transform.position;
        Vector3 peakPos = (startPos + dstPosition) / 2f + Vector3.up * jumpHeight;

        while (jumpTimer < jumpDuration)
        {
            jumpTimer += Time.deltaTime;

            float t = Mathf.Clamp01(jumpTimer / jumpDuration);

            Vector3 newPos = Vector3.Lerp(startPos, peakPos, t) + Mathf.Sin(t * Mathf.PI) * Vector3.up * jumpHeight;
            gameObject.transform.position = newPos;

            yield return null;
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = jump;
        audioSource.Play();

        gameObject.transform.position = dstPosition;

        yield return new WaitForSeconds(jumpDuration);
        onComplete?.Invoke();

    }


    // Helper method for clone effect
    private IEnumerator CloneEffect(GameObject gameObject, Vector3 srcPosition, Vector3 destination, Action onComplete)
    {
        float duration = 1f;
        float time = 0f;
        while (time < duration)
        {
            gameObject.transform.position = Vector3.Lerp(srcPosition, destination, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = clone;
        audioSource.Play();

        gameObject.transform.position = destination;

        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }



}