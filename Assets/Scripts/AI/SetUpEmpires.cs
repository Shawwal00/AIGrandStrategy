using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for spawning in the empires when the game starts. 
*/

public class SetUpEmpires : MonoBehaviour
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

    /*
     * This will spawn all the AI Empires onto the map and set them up
     */ 
    public void SpawnEmpires()
    {
        // Set up the empires depending on how many of them there are.
        int curerntAIOwner = 1;
        EmpireClass copyEmpirePiece;

        List<MapTile> allTilesList = MapBoardScript.ReturnTileList();
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            for (int j = 0; j < allTilesList.Count; j++)
            {
                if (allTilesList[j].GetTileNumber() == spawnPositions[i])
                {
                    copyEmpirePiece = Instantiate(EmpireMangerPrefab, new Vector3(1000,1000,1000),new Quaternion(0,0,0,0));
                    AIMain.AddEmpireToList(copyEmpirePiece);
                    copyEmpirePiece.SetEmpireNumber(curerntAIOwner);
                    empiresInGame.Add(copyEmpirePiece);
                    allTilesList[j].SetOwner(curerntAIOwner);
                    copyEmpirePiece.SetAllTilesList(allTilesList);
                    copyEmpirePiece.EconomyModule.CalculateMoneyUpdateAmount();
                    copyEmpirePiece.WarModule.UpdateReplinishAmount();
                    curerntAIOwner++;
                }
            }
        }

        for (int j = 0; j < allTilesList.Count; j++)
        {
            for (int h = 0; h < empiresInGame.Count; h++)
            {
                allTilesList[j].SetUpAllTileConquerReasons(empiresInGame[h]);
                allTilesList[j].SetUpAllTileMoveReasons(empiresInGame[h]);
            }
        }

        for (int j = 0; j < empiresInGame.Count; j++)
        {
            empiresInGame[j].WarModule.MeetingAllEmpires(empiresInGame);
        }
    }

    /*
     * This can be used to get a refrence to an empire if you know the empire number
     * @param int _empireWanted This is the empire number of the empire you want
     * @return EmpireClass empire This is the empire refrence.
     */ 
    public EmpireClass GetSpecificEmpireClassBasedOnOwner(int _empireWanted)
    {
        foreach (var empire in empiresInGame)
        {
            if (empire.GetEmpireNumber() == _empireWanted)
            {
                return empire;
            }
        }
        return null;
    }
}
