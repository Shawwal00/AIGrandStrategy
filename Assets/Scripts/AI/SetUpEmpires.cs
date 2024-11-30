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
    [SerializeField] public EmpireClass EmpireMangerPrefab;
    [SerializeField] public AIMain AIMain;

    //Variables
    [SerializeField] public List<int> spawnPositions; //This is the spawn positions for the empires - it will also be how many empires you want to spawn.
    private List<EmpireClass> empiresInGame;

    private void Awake()
    {
        empiresInGame = new List<EmpireClass>();
    }

    public void SpawnEmpires()
    {
        // Set up the empires depending on how many of them there are.
        int curerntAIOwner = 1;
        EmpireClass copyEmpirePiece;
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            List<MapTile> allTilesList = MapBoardScript.returnTileList();

            for (int j = 0; j < allTilesList.Count; j++)
            {
                if (allTilesList[j].GetTileNumber() == spawnPositions[i])
                {
                    copyEmpirePiece = Instantiate(EmpireMangerPrefab, new Vector3(1000,1000,1000),new Quaternion(0,0,0,0));
                    empiresInGame.Add(copyEmpirePiece);
                    allTilesList[j].SetOwner(curerntAIOwner);
                    copyEmpirePiece.SetAllTilesList(allTilesList, curerntAIOwner);
                    AIMain.AddEmpireToList(copyEmpirePiece);
                    curerntAIOwner++;
                }
            }
        }
    }
}
