using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard Board;
    [SerializeField] public InputKeys Inputs;
    
    // Setting Initial Position
    private int boardX;
    private int boardY;
    private float cameraHeight;
    
    //Camera Movement
    private float speed = 20;
    
    //Camera Zoom
    private float zoomSpeed = 50;
    
    private void Awake()
    {
        boardX = Board.boardLengthX;
        boardY = Board.boardLengthY;
        cameraHeight = -10;
    }

    public void SetCameraPosition()
    {
        transform.position = new Vector3(  -2 + ((boardX * 1) / 2), -2 + ((boardY * 1) / 2), cameraHeight);
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    public void MoveCamera()
    { 
        //Camera Movement
        transform.Translate(Inputs.movement * Time.deltaTime * speed);
    }
}
