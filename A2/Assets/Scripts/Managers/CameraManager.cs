using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Two modes.
/// Camera always has a focal point on the board.
/// Always a -L away from the focal point, where L is the lookat vector, not normalised.
/// 
/// Normal mode (perspective):
/// Cam  ____________
/// |   \  
/// |    \
/// |     \
/// |      \
/// |       \
///          Focal
///          
/// Tactical mode (orthogonal):
///         Cam
///     /   |   \
///    /    |    \
///   /     |     \
///  /      |      \
///         Focal
///         
/// A Mono so that it can access the mono methods
/// and to avoid problems interacting with the camera.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /*********************************** STATIC ***********************************/

    // Note: in Unity: x is right, y is up, and z is forward.

    public const float cameraDistancePersp = 100.0f;
    public const float cameraDistanceOrtho = 100.0f;
    public const float zoomFactor = 2.0f;

    public static readonly Vector3 lookatVectorPersp = cameraDistancePersp * 
        (Vector3.down + .5f * Vector3.forward).normalized;
    public static readonly Vector3 lookatVectorOrtho = cameraDistanceOrtho * 
        Vector3.down;
    public static readonly Vector3 zoomedLookatVectorPersp = cameraDistancePersp / zoomFactor *
    (Vector3.down + .5f * Vector3.forward).normalized;
    public static readonly Vector3 zoomedLookatVectorOrtho = cameraDistanceOrtho / zoomFactor *
        Vector3.down;

    /*********************************** FIELDS ***********************************/

    // The main camera
    Camera cam;

    // Its focalPoint on the board
    Vector3 focalPoint;

    // true iff the camera is zoomed in.
    bool zoom = false;

    /*********************************** METHODS ***********************************/  

    public Camera getMainCamera() { return cam; }

    /// <returns>true: normal, perspective; false: tactical, orthogonal</returns>
    public bool getMode()
    {
        return cam.orthographic;
    }

    /// <summary>
    /// Changes the mode of the camera.
    /// It changes these and these only
    ///     1. the camera view mode
    ///     2. the camera angle (by changing the lookat vector)
    ///     3. the camera position (by changing the lookat vector)
    /// </summary>
    /// <param name="mode">true: normal, perspective; false: tactical, orthogonal</param>
    public void setMode(bool mode)
    {
        // If already in the mode.
        if(getMode() == mode)
        {
            return;
        }

        // 1. the camera view mode
        cam.orthographic = mode;
        // 2. and 3.
        updateCameraTrans();
    }

    /// <summary>
    /// For now, this should only be called when the camera follows a player's
    /// or an AI's move.
    /// </summary>
    /// <param name="moveVector">
    /// How the focal point should move.
    /// Should be restricted on the x-z plane.
    /// </param>
    public void onFocalPointMove(Vector3 moveVector)
    {
        Utility.MyDebugAssert(moveVector.y == .0f, "Should not move up or down.");

        focalPoint += moveVector;
        updateCameraTrans();
    }

    /// <summary>
    /// zoom the camera from 1.0f to zoomFactor.
    /// Has no effect if the camera is already zoomed in.
    /// </summary>
    public void zoomIn()
    {
        zoom = true;
        updateCameraTrans();
    }

    /// <summary>
    /// zoom the camera to 1.0f
    /// Has no effect if the camera is already zoomed out.
    /// </summary>
    public void zoomOut()
    {
        zoom = false;
        updateCameraTrans();
    }

    /*********************************** PRIVATE HELPERS ***********************************/

    /// <summary>
    /// Called after a
    ///     1. mode change, or
    ///     2. a focal point change
    ///     3. a zoom change
    /// </summary>
    private void updateCameraTrans()
    {
        if (getMode())
        // perspective
        {
            cam.transform.position = focalPoint - 
                (zoom ? zoomedLookatVectorPersp : lookatVectorPersp);
        }
        else
        // orthogonal
        {
            cam.transform.position = focalPoint -
                (zoom ? zoomedLookatVectorOrtho : lookatVectorOrtho);
        }
        cam.transform.LookAt(focalPoint);
    }

    /*********************************** MONO ***********************************/
    /// <summary>
    /// Obtain the camera,
    /// and set up it with the boardd
    /// </summary>
    void Awake()
    {
        // This is the XR Rig -> Camera Offset -> Main Camera
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Utility.MyDebugAssert(cam != null, "should not be null");

        // the initial focal point is the center of the board.
        focalPoint = Game.gameSingleton.modelMgr.getBoardSurfaceCenter();

        // the default mode is the perspective mode.
        // setup the camera transform.
        if(getMode())
        {
            // if the mode is already true,
            // then I can't call setMode(), as it will skip the call then.
            updateCameraTrans();
        }
        else
        {
            setMode(true);
        }
    }

    /// <summary>
    /// Not used for now.
    /// </summary>
    void Update()
    {
        
    }
}
