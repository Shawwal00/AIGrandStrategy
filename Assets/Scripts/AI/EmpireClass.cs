using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for the indivigual empires that will be used.
 * It will contain all the parameters that the ai needs to function.
*/

public class EmpireClass : MonoBehaviour
{

    private GameObject GameManager;
    private MapBoard MapBoardScript;

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private int EmpireNumber = 0;

    private void Awake()
    {
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();
        GameManager = GameObject.Find("GameManager");
        MapBoardScript = GameManager.GetComponent<MapBoard>();
    }

    //The below function is used when the AI captures a new tile
    public void ConquerTerritory()
    {
        GetExpandingTilesOfTile();
        int randomNumber = Random.Range(0, expandingTiles.Count);
        if (expandingTiles.Count > 0)
        {
            expandingTiles[randomNumber].SetOwner(EmpireNumber);
        }
    }

    //The below function is used to get a list of all the tiles when the map is set up and also set the empire number.
    public void SetAllTilesList(List<MapTile> _newTileList, int _empireNumber)
    {
        allTilesList = _newTileList;
        EmpireNumber = _empireNumber;
        GetOwnedTiles();
    }

    //The below function is used to get tiles that are owned by this empire specifically.
    public void GetOwnedTiles()
    {
        ownedTiles.Clear();
        for (int i = 0; i < allTilesList.Count; i++)
        {
            if (allTilesList[i].GetOwner() == EmpireNumber)
            {
                ownedTiles.Add(allTilesList[i]);
            }
        }
    }

    //The below function will get the tiles that are adjacent to a tile and put them into a list
    public void GetExpandingTilesOfTile()
    {
        expandingTiles.Clear();
        for (int i = 0; i < ownedTiles.Count; i++)
        {
          List<MapTile> tempList = ownedTiles[i].GetAllConnectedTiles();
            for (int j = 0; j < tempList.Count; j++)
            {
                if (tempList[j].GetOwner() != EmpireNumber)
                {
                    expandingTiles.Add(tempList[j]);
                }
            }
        }
    }

    //Return the empire number
    public int GetEmpireNumber()
    {
        return EmpireNumber;
    }
}
