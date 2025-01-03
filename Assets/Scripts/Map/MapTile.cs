using System;
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

    //Variables
    private List<MapTile> allConnectedTiles; //All the tiles that are adjacent to this tile - can only move up, down and sideways
    private int tileNumber; // This is what tile number the map is.

    private int owner = 0; // 0 is neutral, //1 - 4 is ai

    private int troopPresent = 0; // This is how many troops are present within the territory
    private int troopAdding = 0;  // This is how many troops will be added to the provinence every second

    private void Awake()
    {
        allConnectedTiles = new List<MapTile>();
        GameManager = GameObject.Find("GameManager");
        TileData = GameManager.GetComponent<TileData>();
    }

    //Add tiles to which units can move towards
    public void AddConnectionTile(MapTile _newConnection)
    {
        allConnectedTiles.Add(_newConnection);
    }

    //This return all the adjacent tiles
    public List<MapTile> GetAllConnectedTiles()
    {
        return allConnectedTiles;
    }

    //Returns the tile number of the tile
    public int GetTileNumber()
    {
        return tileNumber;
    }

    // Sets the tile number which is the position of the tile on the grid also sets up the tile data using the TileData script
    public void SetTileNumber(int _number)
    {
        tileNumber = _number;
        Dictionary<int, Dictionary<string, int>> allTileData = TileData.GetTileData();
        SetTroopPresent(allTileData[tileNumber]["Present"]);
        SetTroopAdding(allTileData[tileNumber]["Replenish"]);

    }

    //Sets the owner of the tile which will always be one of the AIs
    public void SetOwner(int _newOwner)
    {
        owner = _newOwner;
        if (owner == 1)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (owner == 2)
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    //Gets the current empire owner of the tile
    public int GetOwner()
    {
        return owner;
    }

    //Sets the amount of troop on the tile
    public void SetTroopPresent(int _troopPresent)
    {
        troopPresent = _troopPresent;
    }

    //Gets the amount of troop on the tile
    public int GetTroopPresent()
    {
        return troopPresent;
    }

    //Sets the amount of troops that will be added to the controlling empire.
    public void SetTroopAdding(int _troopAdding)
    {
        troopAdding = _troopAdding;
    }

    //Gets the amount of troops that will be added to the empire.
    public int GetTroopAdding()
    {
        return troopAdding;
    }
}
