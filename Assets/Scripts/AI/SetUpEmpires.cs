using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for spawning in the empires when the game starts. 
*/

public class SetUpEmpire : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard MapBoardScript;

     //Variables
    [SerializeField] public List<int> spawnPositions; //This is the spawn positions for the empires - it will also be how many empires you want to spawn.

    public void SpawnEmpires()
    {
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            List<MapTile> allTilesList = MapBoardScript.returnTileList();

            for (int j = 0; j < allTilesList.Count; j++)
            {
                if (allTilesList[j].ReturnTileNumber() == spawnPositions[i])
                {
                    allTilesList[j].SetOwner(1);
                    Debug.Log("safsasf");
                }
            }
        }
    }
}
