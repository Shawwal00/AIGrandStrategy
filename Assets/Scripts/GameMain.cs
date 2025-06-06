using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * This script is the overall manger of all of the other scripts - this is to ensure easy testing if something stops working. 
*/

public class GameMain : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard Board;
    [SerializeField] public CameraController cameraController;
    [SerializeField] public SetUpEmpires EmpireSetUp;
    [SerializeField] public Cursor cursor;

    private void Start()
    {
        Board.CreateMap();
        EmpireSetUp.SpawnEmpires();
    }

    private void Update()
    {
        cameraController.MoveCamera();
        cursor.MouseMovement();
        cursor.TileCursor();
    }
}
