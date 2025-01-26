using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*
 * This script is responsible for the indivigual empires that will be used.
 * It will contain all the parameters that the ai needs to function.
*/

public class EmpireClass : MonoBehaviour
{

    private GameObject GameManager;
    private MapBoard MapBoardScript;
    private AIMain AiMain;

    public DiplomacyModule DiplomacyModule;
    public WarModule WarModule;

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private int empireNumber = 0;

    private Color empireColor = Color.white;

    private void Awake()
    {
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();
        GameManager = GameObject.Find("GameManager");
        MapBoardScript = GameManager.GetComponent<MapBoard>();
        AiMain = GameManager.GetComponent<AIMain>();

        DiplomacyModule = this.AddComponent<DiplomacyModule>();
        WarModule = this.AddComponent<WarModule>();

        WarModule.SetThisEmpire(this);
        DiplomacyModule.SetThisEmpire(this);
    }

    //The below function is used to get a list of all the tiles when the map is set up.
    public void SetAllTilesList(List<MapTile> _newTileList)
    {
        allTilesList = _newTileList;
        GetOwnedTiles();
    }

    //The below function is used to get tiles that are owned by this empire specifically.
    public void GetOwnedTiles()
    {
        ownedTiles.Clear();
        for (int i = 0; i < allTilesList.Count; i++)
        {
            if (allTilesList[i].GetOwner() == empireNumber)
            {
                ownedTiles.Add(allTilesList[i]);
            }
        }
        if (ownedTiles.Count == 0)
        {
            AiMain.EmpireDestroyed(this);
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
                if (tempList[j].GetOwner() != empireNumber)
                {
                    expandingTiles.Add(tempList[j]);
                }
            }
        }
    }

    // This function will set the new empire number
    public void SetEmpireNumber(int _newEmpireNumber)
    {
        empireNumber = _newEmpireNumber;
    }

    //Return the empire number
    public int GetEmpireNumber()
    {
        return empireNumber;
    }

    //The below function will set the empire color for the tiles
    public void SetEmpireColor(Color newEmpireColor)
    {
        empireColor = newEmpireColor;
    }

    //The below function will return the empire color used for the tiles
    public Color GetEmpireColor()
    {
        return empireColor;
    }

    // The below will return all of the owned tiles
    public List<MapTile> ReturnOwnedTiles()
    {
        return ownedTiles; 
    }

    // The below will return all of the expanding tiles
    public List<MapTile> ReturnExpandingTiles()
    {
        return expandingTiles;
    }

    // The below will return all of the tiles
    public List<MapTile> ReturnAllTiles()
    {
        return allTilesList;
    }
}
