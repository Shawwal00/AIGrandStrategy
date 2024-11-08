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
     [SerializeField] public MapBoard board;

    private void Start()
    {
        board.CreateMap();
    }

    private void Update()
    {
        
    }
}
