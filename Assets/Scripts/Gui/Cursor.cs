using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Cursor : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard board;
    
    //Variables
    private Vector3 mousePosition;
    private GameObject tileCursor;
    private Vector3 tileCursorPosition;
    private int currentTileCount;
    private void Awake()
    {
        tileCursor = GameObject.Find("TileCursor");
        
        transform.position = new Vector3(0, 2, 0);
    }
    
    public void MouseMovement()
    {
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = 10f; /* This has to be set manually otherwise it is 0. The Z controls how close the mouse is
         to the camera, 9.45 was the best I found */
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = mousePosition;
    }

    //Sets the position of the tile cursor and ensure that it snaps to the grid
    public void TileCursor()
    {
        currentTileCount = board.ReturnTileList().Count;
        foreach (var tiles in board.ReturnTileList())
        {
            currentTileCount -= 1;
            if (tiles.transform.position.x + board.ReturnRenderer().bounds.size.y / 2 >= transform.position.x &&
                tiles.transform.position.x - board.ReturnRenderer().bounds.size.y / 2 < transform.position.x && 
                tiles.transform.position.y + board.ReturnRenderer().bounds.size.y / 2 >= transform.position.y &&
                tiles.transform.position.y - board.ReturnRenderer().bounds.size.y / 2 < transform.position.y)
            {
                tileCursorPosition = tiles.transform.position;
                tileCursorPosition.z = -0.55f; // This is to set it depending on the camera
                tileCursor.transform.position = tileCursorPosition;
                currentTileCount = board.ReturnTileList().Count;
            }

            // If the cursor is not on the grid then the tile cursor will go to 0,0,0
            else if (currentTileCount == 0)
            {
                tileCursorPosition = new Vector3(0,0,0);
                tileCursor.transform.position = tileCursorPosition;
                currentTileCount = 5000;
            }
        }
    }

    //Returns the tile which the cursor is currently on 
    public int ReturnLastTile()
    {
        return currentTileCount - 1;
    }
}
 