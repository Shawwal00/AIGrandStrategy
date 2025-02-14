
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * This script is a the MapTile class that is each individual tile and contains its data.
*/

public class MapTile : MonoBehaviour
{
    private GameObject GameManager;

    //Scripts
    private TileData TileData;
    private SetUpEmpires SetUpEmpires;

    //Variables
    private List<MapTile> allConnectedTiles; //All the tiles that are adjacent to this tile - can only move up, down and sideways
    private int tileNumber; // This is what tile number the map is.

    private int owner = 0; // 0 is neutral, //1 - 4 is ai
    public int income = 0; // The tiles income

    private int troopPresent = 0; // This is how many troops are present within the territory
    private int troopAdding = 0;  // This is how many troops will be added to the provinence every second

    private void Awake()
    {
        allConnectedTiles = new List<MapTile>();
        GameManager = GameObject.Find("GameManager");

        //Scripts
        TileData = GameManager.GetComponent<TileData>();
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();
    }

    /*
     * Add tiles to which units can move towards
     * @param MapTile _newConnection This is the tile that is a new connection
     */
    public void AddConnectionTile(MapTile _newConnection)
    {
        allConnectedTiles.Add(_newConnection);
    }

    /*
     * This return all the adjacent tiles
     * @return List<MapTile> allConnectedTiles This is all of the connected tiles of a tile
     */
    public List<MapTile> GetAllConnectedTiles()
    {
        return allConnectedTiles;
    }

    /*
     * Returns the tile number of the tile
     * @return int tileNumber This is the tile number
     */
    public int GetTileNumber()
    {
        return tileNumber;
    }

    /*
     * Sets the tile number which is the position of the tile on the grid also sets up the tile data using the TileData script
     * @param int _number Sets the tile to this number
     */
    public void SetTileNumber(int _number)
    {
        tileNumber = _number;
        Dictionary<int, Dictionary<string, int>> allTileData = TileData.GetTileData();
        if (allTileData.ContainsKey(tileNumber) == false)
        {
            SetTroopPresent(Random.Range(15, 25));
            SetTroopAdding(Random.Range(3, 5));
            SetIncome(Random.Range(20, 40));
        }
        else 
        {
            SetTroopPresent(allTileData[tileNumber]["Present"]);
            SetTroopAdding(allTileData[tileNumber]["Replenish"]);
        }

    }

    /*
     * Sets the owner of the tile which will always be one of the AIs
     * @param _newOwner This is the new owner of the tile
     */
    public void SetOwner(int _newOwner)
    {
        owner = _newOwner;
        EmpireClass empire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(owner);
        if (empire.GetEmpireColor() == Color.white)
        {
            if (owner == 1)
            {
                GetComponent<MeshRenderer>().material.color = Color.red;
                empire.SetEmpireColor(Color.red);
            }
            else if (owner == 2)
            {
                GetComponent<MeshRenderer>().material.color = Color.blue;
                empire.SetEmpireColor(Color.blue);
            }
            else if (owner == 3)
            {
                GetComponent<MeshRenderer>().material.color = Color.black;
                empire.SetEmpireColor(Color.black);
            }
            else if (owner == 4)
            {
                GetComponent<MeshRenderer>().material.color = Color.green;
                empire.SetEmpireColor(Color.green);
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = empire.GetEmpireColor();
        }
    }

    /*
     * Gets the current empire owner of the tile
     * @return int owner This is the owner of the tile
     */
    public int GetOwner()
    {
        return owner;
    }

    /*
     * Sets the amount of troop on the tile
     * @param int _troopPresent This is the number of troops that will now be present
     */
    public void SetTroopPresent(int _troopPresent)
    {
        troopPresent = _troopPresent;
    }

    /*
     * Gets the amount of troop on the tile
     * @return int troopPresent This is the amount of troops present on the tile
     */
    public int GetTroopPresent()
    {
        return troopPresent;
    }

    /*
     * Sets the amount of troops that will be added to the controlling empire.
     * @param int _troopAdding This is what the new troopAdding will be
     */
    public void SetTroopAdding(int _troopAdding)
    {
        troopAdding = _troopAdding;
    }

    /*
     * Gets the amount of troops that will be added to the empire.
     * @return int troopAdding This is the amount of troops being added
     */
    public int GetTroopAdding()
    {
        return troopAdding;
    }

    /*
     * The below function is used to set the income
     * @param int _newIncome This is the value that the income of this tile will now be set to
     */
    public void SetIncome(int _newIncome)
    {
        income = _newIncome;
    }

    /*
     * The below function is used to get the current income of this tile
     * @return int income This is the income of the tile
     */ 
    public int GetIncome()
    {
        return income;
    }
}
