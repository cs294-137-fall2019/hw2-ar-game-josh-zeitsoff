
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Adding OnTouch3D here forces us to implement the 
// OnTouch function, but also allows us to reference this
// object through the OnTouch3D class.
public class Button : MonoBehaviour, OnTouch3D
{
    // Debouncing is a term from Electrical Engineering referring to 
    // preventing multiple presses of a button due to the physical switch
    // inside the button "bouncing".
    // In CS we use it to mean any action to prevent repeated input. 
    // Here we will simply wait a specified time before letting the button
    // be pressed again.
    // We set this to a public variable so you can easily adjust this in the
    // Unity UI.
    public float debounceTime = 0.1f;
    // Stores a counter for the current remaining wait time.
    private float remainingDebounceTime;

    public GameObject prefabX;
    public GameObject prefabO;
    public Text positionText;
    public ARButtonManager manager;
    private bool placed;
    public GameObject created;

    void Start()
    {
        remainingDebounceTime = 0;
        //remainingDebounceTime = 0;
    }

    void Update()
    {
        // Time.deltaTime stores the time since the last update.
        // So all we need to do here is subtract this from the remaining
        // time at each update.
        if (remainingDebounceTime > 0) {
            remainingDebounceTime -= Time.deltaTime;
        }
        

    }

    public void OnTouch()
    {
        if (manager.getGameBoard().remainingDebounceTime > 0)
        {
            manager.getGameBoard().remainingDebounceTime -= Time.deltaTime;
            return;

        }


        //positionText.gameObject.SetActive(true);
        positionText.text = "(" + transform.position.x + "," +
            transform.position.y + "," + transform.position.z + "," + manager.player() + ")";

        // If a touch is found and we are not waiting,
        if (remainingDebounceTime <= 0)
        {
            if (!placed)
            {
                // Place X or O slightly above board     
                Vector3 newPos = new Vector3(transform.position.x, (float)(transform.position.y + .01), transform.position.z);
                created = Instantiate(manager.player() ? prefabX : prefabO, newPos
                    , Quaternion.Euler(90f, 0f, 0f));

                //register touch before changing player
                manager.registerButton(newPos);

                manager.changePlayer();
                remainingDebounceTime = debounceTime;
                placed = true;
            }
        }
    }

    public void unplace() {
        placed = false;
    }


}
