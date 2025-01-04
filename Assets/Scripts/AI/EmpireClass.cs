using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * This script is responsible for the indivigual empires that will be used.
 * It will contain all the parameters that the ai needs to function.
*/

public class EmpireClass : MonoBehaviour
{

    private GameObject GameManager;
    private MapBoard MapBoardScript;
    private SetUpEmpires SetUpEmpires;

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private int EmpireNumber = 0;

    private int troopNumber = 0;
    private float updateTroopNumberTime = 0;

   // private bool boarderingEmpire = false;

    private List<EmpireClass> boarderingEmpires;

    private void Awake()
    {
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();
        boarderingEmpires = new List<EmpireClass>();
        GameManager = GameObject.Find("GameManager");
        MapBoardScript = GameManager.GetComponent<MapBoard>();
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();
    }

    private void Update()
    {
        UpdateAllTroopCount();
    }

    //This gets all the tiles owned by the empire and adds to the total empire troop count.
    private void UpdateAllTroopCount()
    {
        updateTroopNumberTime += Time.deltaTime;
        if (ownedTiles.Count > 0)
        {
            if (updateTroopNumberTime > 1)
            {
                for (int i = 0; i < ownedTiles.Count; i++)
                {
                    AddToTroopNumber(ownedTiles[i].GetTroopAdding());
                }
                updateTroopNumberTime = 0;
            }
        }
    }

    //The below function is used when the AI captures a new tile
    public void ConquerTerritory()
    {
        GetExpandingTilesOfTile();
        int randomNumber = Random.Range(0, expandingTiles.Count);
        if (expandingTiles.Count > 0)
        {
            MapTile lowestTile = null;

            foreach (MapTile tile in expandingTiles)
            {
                if (lowestTile == null)
                {
                    if (tile.GetOwner() == 0)
                    {
                        lowestTile = tile;
                    }
                }
                else
                {
                    if (tile.GetOwner() == 0)
                    {
                        if (tile.GetTroopPresent() < lowestTile.GetTroopPresent())
                        {
                            lowestTile = tile;
                        }
                    }
                }
            }
            if (lowestTile != null && lowestTile.GetTroopPresent() < troopNumber)
            {
                if (EmpireNumber == 1)
                {
                    Debug.Log(lowestTile.GetTileNumber());
                }
                // Debug.Log(troopNumber);
                // Debug.Log(lowestTile.GetTroopPresent());
                lowestTile.SetOwner(EmpireNumber);
                SetTroopNumber(troopNumber - lowestTile.GetTroopPresent());

                List<MapTile> mapTiles = lowestTile.GetAllConnectedTiles();
                foreach (MapTile tile in mapTiles)
                {
                    if (tile.GetOwner() != 0)
                    {
                        bool empireFound = false;
                        foreach (var empire in boarderingEmpires)
                        {
                            if (empire.EmpireNumber == tile.GetOwner())
                            {
                                empireFound = true;
                            }
                        }
                        if (empireFound == false)
                        {
                            boarderingEmpires.Add(SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner()));
                            Debug.Log("Working");
                        }
                        break;
                    }
                }
                // Debug.Log(troopNumber);
            }
        }
    }

    //The below function is used to check if the AI should go to war with a neighbouring faction
    public void GoToWarCheck()
    {

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

    //This will add to the empires troop number
    public void AddToTroopNumber(int _addTroopNumber)
    {
        troopNumber += _addTroopNumber;
    }

    //The below will set the empires troop number
    public void SetTroopNumber(int _setTroopNumber)
    {
        troopNumber = _setTroopNumber;
    }

    //The below function will return the boardering empire value
    public List<EmpireClass> GetBoarderingEmpire()
    {
        return boarderingEmpires;
    }
}
