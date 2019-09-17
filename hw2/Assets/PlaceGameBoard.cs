
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// These allow us to use the ARFoundation API.
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceGameBoard : MonoBehaviour
{
    // Public variables can be set from the unity UI.
    // We will set this to our Game Board object.
    public GameObject gameBoard;
    // These will store references to our other components.
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    // This will indicate whether the game board is set.
    private bool placed = false;
    public float debounceTime = .2f;
    public float remainingDebounceTime;
    private Vector3 boardCenter;
    private float resetCD = .2f;

    // Start is called before the first frame update.
    void Start()
    {
        // GetComponent allows us to reference other parts of this game object.
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        resetCD = 0;
    }

    // Update is called once per frame.
    void Update()
    {
        if (resetCD > 0) {
            resetCD -= Time.deltaTime;
            return;
        }
        if (!placed)
        {
            if (Input.touchCount > 0)
            {
                Vector2 touchPosition = Input.GetTouch(0).position;

                // Raycast will return a list of all planes intersected by the
                // ray as well as the intersection point.
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(
                    touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    // The list is sorted by distance so to get the location
                    // of the closest intersection we simply reference hits[0].
                    var hitPose = hits[0].pose;
                    // Now we will activate our game board and place it at the
                    // chosen location.
                   
                    gameBoard.SetActive(true);
                    remainingDebounceTime = debounceTime;
                    gameBoard.transform.position = hitPose.position;
                    boardCenter = gameBoard.transform.position;

                    placed = true;

                    for (int i = 0; i < gameBoard.transform.childCount; i++)
                    {
                        gameBoard.transform.GetChild(i).transform.GetChild(0)
                            .gameObject.SetActive(true);
                    }




                    // After we have placed the game board we will disable the
                    // planes in the scene as we no longer need them.
                    planeManager.SetTrackablesActive(false);

                }
            }
        }
        else
        {
            // The plane manager will set newly detected planes to active by 
            // default so we will continue to disable these.
            planeManager.SetTrackablesActive(false);
        }
    }

    // If the user places the game board at an undesirable location we 
    // would like to allow the user to move the game board to a new location.
    public void AllowMoveGameBoard()
    {
        placed = false;
        gameBoard.SetActive(false);
        for (int i = 0; i < gameBoard.transform.childCount; i++)
        {
            gameBoard.transform.GetChild(i).transform.GetChild(0)
                .gameObject.SetActive(false);
            gameBoard.transform.GetChild(i).transform.GetChild(0).GetComponent<Button>().unplace();
            Destroy(gameBoard.transform.GetChild(i).transform.GetChild(0).GetComponent<Button>().created) ;
        }
        resetCD = .2f;
        planeManager.SetTrackablesActive(true);
    }

    // Lastly we will later need to allow other components to check whether the
    // game board has been places so we will add an accessor to this.
    public bool Placed()
    {
        return placed;
    }

    public Vector3 boardCenterPos() {
        return boardCenter;
    }

  

    
}
