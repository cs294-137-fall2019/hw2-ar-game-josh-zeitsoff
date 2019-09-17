
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARButtonManager : MonoBehaviour
{
    private Camera arCamera;
    private PlaceGameBoard placeGameBoard;
    private bool turn = true;
    private int[,] board = { { -1, -2, -3 }, { -4, -5, -6 }, { -7, -8, -9 } }; //2 is X, 1 is O
    public Text winnerText;
    private int numPlaced;

    void Start()
    {
        // Here we will grab the camera from the AR Session Origin.
        // This camera acts like any other camera in Unity.
        arCamera = GetComponent<ARSessionOrigin>().camera;
        // We will also need the PlaceGameBoard script to know if
        // the game board exists or not.
        placeGameBoard = GetComponent<PlaceGameBoard>();
    }

    void Update()
    {

        if (!gameOver() && placeGameBoard.Placed() && Input.touchCount > 0)
        {


            Vector2 touchPosition = Input.GetTouch(0).position;
            // Convert the 2d screen point into a ray.
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            // Check if this hits an object within 100m of the user.
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                // Check that the object is interactable.
                if (hit.transform.tag == "Interactable")
                {
                    // Call the OnTouch function.
                    // Note the use of OnTouch3D here lets us
                    // call any class inheriting from OnTouch3D.
                    hit.transform.GetComponent<OnTouch3D>().OnTouch();


                }
            }
        }
    }

    //return true for X, false for O
    public bool player()
    {
        return turn;
    }

    public void changePlayer()
    {
        turn = !turn;
    }

    public PlaceGameBoard getGameBoard()
    {
        return placeGameBoard;
    }

    public void registerButton(Vector3 newPos)
    {
        numPlaced += 1;
        float centerX = placeGameBoard.boardCenterPos().x;
        float centerZ = placeGameBoard.boardCenterPos().z;
        float newPosX = newPos.x;
        float newPosZ = newPos.z;

        //i for row, j for col, access board[i, j]
        // x is left right, z is up down
        //x for col, z for row
        int j = newPosZ > centerZ ? 2 : System.Math.Abs(newPosZ - centerZ) < .00001 ? 1 : 0;
        int i = newPosX > centerX ? 2 : System.Math.Abs(newPosX - centerX) < .00001 ? 1 : 0;

        board[i, j] = turn ? 2 : 1; // turn is true -> X, else O
        print(gameOver());
        if (gameOver())
        {
            //set winner, set button to active
            winnerText.gameObject.SetActive(true);
            winnerText.text = "Player " + (turn ? "X" : "O") + " Won !";

        }
        else if (numPlaced == 9) {
            winnerText.gameObject.SetActive(true);
            winnerText.text = "It's a tie!";
        }
    }

    public bool gameOver()
    {
        bool leftDown = (board[0, 0] == board[1, 0]) && (board[1, 0] == board[2, 0]);
        bool middleDown = (board[0, 1] == board[1, 1]) && (board[1, 1] == board[2, 1]);
        bool rightDown = (board[0, 2] == board[1, 2]) && (board[1, 2] == board[2, 2]);

        bool topAcross = (board[0, 0] == board[0, 1]) && (board[0, 1] == board[0, 2]);
        bool middleAcross = (board[1, 0] == board[1, 1]) && (board[1, 1] == board[1, 2]);
        bool bottomAcross = (board[2, 0] == board[2, 1]) && (board[2, 1] == board[2, 2]);

        bool bLtRDiag = (board[2, 0] == board[1, 1]) && (board[1, 1] == board[0, 2]); //bottom left to top right
        bool tLbRDiag = (board[0, 0] == board[1, 1]) && (board[1, 1] == board[2, 2]); //top left to bottom right

        bool down = leftDown || middleDown || rightDown;
        bool across = topAcross || middleAcross || bottomAcross;
        bool diag = bLtRDiag || tLbRDiag;
        return down || across || diag;
    }


    public void resetBoard()
    {
        winnerText.gameObject.SetActive(false);
        board = new int[,] { { -1, -2, -3 }, { -4, -5, -6 }, { -7, -8, -9 } }; //2 is X, 1 is O}
        numPlaced = 0;
        placeGameBoard.AllowMoveGameBoard();
    }
}
