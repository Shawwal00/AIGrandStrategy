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

    private GameObject GameManager; // This is the object that contains all the scripts.
    private MapBoard MapBoardScript; // This sets up the game map
    private AIMain AiMain; // Everything related to multiple AI will happen in here

    public DiplomacyModule DiplomacyModule; // Handles everything related to diplomacy
    public WarModule WarModule; // Handles everyrything related to war
    public EconomyModule EconomyModule; // Handles everything related to the economy

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private List<MapTile> allTilesWithBuildings; // This is a list of all the tiles which have a building

    private int empireNumber = 0;

    private Color empireColor = Color.white; // Is default to white but will be changed to another colour

    private void Awake()
    {
        // Lists
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();

        //Gameobjects
        GameManager = GameObject.Find("GameManager");

        //Scripts
        MapBoardScript = GameManager.GetComponent<MapBoard>();
        AiMain = GameManager.GetComponent<AIMain>();

        //Modules
        DiplomacyModule = this.AddComponent<DiplomacyModule>();
        WarModule = this.AddComponent<WarModule>();
        EconomyModule = this.AddComponent<EconomyModule>();

        WarModule.SetThisEmpire(this);
        DiplomacyModule.SetThisEmpire(this);
        EconomyModule.SetThisEmpire(this);
    }

    /*
     *The below function is used to set a list of all the tiles. 
     *@param _newTileList This is the new list that  will replace the old tile list
     */

    public void SetAllTilesList(List<MapTile> _newTileList)
    {
        allTilesList = _newTileList;
        UpdateOwnedTiles();
    }

    /*
     * The below function will return the current safest mine tile that the empire has
     */
    public MapTile GetSafestMineTile()
    {

    }

    /*
     * The below function will get the tiles that boarder another empire
     */ 
    public MapTile GetBoarderTilesWithThreateningEmpire()
    {

    }

    /*
     * The below function will return the safest plane tile that the empire has
     */
    public MapTile GetSafestPlaneTile()
    {

    }


    /*
     * The below function is used to get tiles that are owned by this empire specifically.
     */
    public void UpdateOwnedTiles()
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

    /*
     * The below function will get the tiles that are adjacent to a tile and put them into a list
     */
    public void UpdateExpandingTilesOfTile()
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

    /* 
     * This function will set the new empire number
     * @param int _newEmpireNumber This is the new empire number
     */
    public void SetEmpireNumber(int _newEmpireNumber)
    {
        empireNumber = _newEmpireNumber;
    }

    /*
     * Return the empire number
     * @return int empireNumber This is the empire number
     */ 
    public int GetEmpireNumber()
    {
        return empireNumber;
    }

    /*
     * The below function will set the empire color for the tiles
     * @param Color _newEnpireColor This is the new empire colour
     */
    public void SetEmpireColor(Color _newEmpireColor)
    {
        empireColor = _newEmpireColor;
    }

    /*
     * The below function will return the empire color used for the tiles
     * @return Color empireColor This is the empire colour
     */
    public Color GetEmpireColor()
    {
        return empireColor;
    }

    /*
     * The below will return all of the owned tiles
     * @return List<MapTile> ownedTiles This is all of the owned tiles
     */
    public List<MapTile> GetOwnedTiles()
    {
        return ownedTiles; 
    }

    /*
     * The below will return all of the expanding tiles
     * @return List<MapTile> expandingTiles This is all of the expanding tiles
     */
    public List<MapTile> GetExpandingTiles()
    {
        return expandingTiles;
    }

    /*
     * The below will return all of the tiles
     * @return List<MapTile> alTilesList This is a list of all the tiles
     */
    public List<MapTile> GetAllTiles()
    {
        return allTilesList;
    }
}
