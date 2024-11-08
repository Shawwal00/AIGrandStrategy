using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * This script is a the MapTile class that is each individual tile and contains its data.
*/

public class MapTile : MonoBehaviour
{
    //Variables
    private List<MapTile> allConnectedTiles; //All the tiles that are adjacent to this tile - can only move up, down and sideways
    private int tileCost; // The cost to travel to the tile 

    private void Awake()
    {
        allConnectedTiles = new List<MapTile>();
    }

    //Add tiles to which units can move towards
    public void AddConnectionTile(MapTile newConnection)
    {
        allConnectedTiles.Add(newConnection);
    }

    //This return all the adjacent tiles
    public List<MapTile> ReturnAllConnectedTiles()
    {
        return allConnectedTiles;
    }

    //Sets the tile costs off all the adjacent tiles
    public void SetConnectionTileCosts()
    {
        for (int i = 0; i < allConnectedTiles.Count; i++)
        {
            allConnectedTiles[i].tileCost = tileCost + 1;
        }
    }

    //Sets the tile cost
    public void SetTileCost(int costValue)
    {
        tileCost = costValue;
    }

    //Returns the tile cost
    public int ReturnTileCost()
    {
        return tileCost;
    }
}
