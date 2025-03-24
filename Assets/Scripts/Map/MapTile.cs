
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * This script is a the MapTile class that is each individual tile and contains its data.
*/

public class MapTile : MonoBehaviour
{

    public BuildingData buildingData;

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
    private int currentPopulation = 0; // The population within this tile
    private int addingPopulation = 0; // The amount of population that is added to this tile
    private int currentAmeneties = 0; // This is the current amount of ameneties that is present in this population

    private int corruptPopulation = 0; // This is the amount of corrupt population that is within this tile

    public enum TileType { None, Plain, Mine };
    public TileType thisTileType = TileType.None;

    private Dictionary<EmpireClass,Dictionary< string, int>> conquerTileReasons = new Dictionary<EmpireClass, Dictionary<string, int>>(); // These are the reasons that this tile may be conquered.
    private Dictionary<EmpireClass, Dictionary<string, int>> buildFortReasons = new Dictionary<EmpireClass, Dictionary<string, int>>(); // These are the reasons that this may have a fort built on it.


    private void Awake()
    {
        allConnectedTiles = new List<MapTile>();
        GameManager = GameObject.Find("GameManager");

        //Scripts
        TileData = GameManager.GetComponent<TileData>();
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();

        buildingData = this.AddComponent<BuildingData>();

    }


    /*
    * The below function is used so that when the empires are set up the tiles will set up all the reasons to conquer for each empire
    * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
    */
    public void SetUpAllTileBuildReasons(EmpireClass _otherEmpire)
    {
        buildFortReasons[_otherEmpire] = new Dictionary<string, int>();
        buildFortReasons[_otherEmpire]["Garrison"] = 0; // This is how much of a garrison the tile has
        buildFortReasons[_otherEmpire]["TileReplenish"] = 0; // This is how fast the tile replenished
        buildFortReasons[_otherEmpire]["Income"] = 0; // This is how much the tile produces
    }


    /*
    * The below will loop through all the reasons and update the reasons to build a fort on a specific tile
    * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
    * @retun int total This is all the added up reasons to conquer this tile
    */
    public int UpdateBuildFortForAllTiles(EmpireClass _otherEmpire)
    {
        int total = 0;
        total += buildFortReasons[_otherEmpire]["Garrison"];
        total += buildFortReasons[_otherEmpire]["TileReplenish"];
        total += buildFortReasons[_otherEmpire]["Income"];

        return total;
    }

    /*
    * The below function is used to update the value for a reason why to build a fort has increased or decreased.
    * @param string _reason This is the reason that the tile reason is increasing or decreasing
    * @param int _newValue This is the new value that it will be set to.s
    * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
    */
    public void ChangeValueInBuildFort(string _reason, int _newValue, EmpireClass _otherEmpire)
    {
        buildFortReasons[_otherEmpire][_reason] = _newValue;
    }

    /*
     * The below function is used so that when the empires are set up the tiles will set up all the reasons to conquer for each empire
     * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
     */
    public void SetUpAllTileConquerReasons(EmpireClass _otherEmpire)
    {
        conquerTileReasons[_otherEmpire] = new Dictionary<string, int>();
        conquerTileReasons[_otherEmpire]["Boardering"] = 0; // This is if the tile is boardering another empire
        conquerTileReasons[_otherEmpire]["Garrison"] = 0; // This is how much of a garrison the tile has
        conquerTileReasons[_otherEmpire]["YourTroops"] = 0; // This is the empires troops and if it has enough to take the tile or if at war to win against the warring empire - should not take tiles if it will lose
        conquerTileReasons[_otherEmpire]["TileReplenish"] = 0; // This is how fast the tile replenished
        conquerTileReasons[_otherEmpire]["Income"] = 0; // This is how much the tile produces
        conquerTileReasons[_otherEmpire]["EmpireConquer"] = 0; // This is how likely the it is that the AI will conquer the tile before you
        conquerTileReasons[_otherEmpire]["Attacked"] = 0; // This is how likely it is that the empire will be attacked
        conquerTileReasons[_otherEmpire]["ImportantTile"] = 0; // This is if the tile is an important type
        conquerTileReasons[_otherEmpire]["MineBuilt"] = 0; // This is if there is a mine built on this tile
        conquerTileReasons[_otherEmpire]["BarracksBuilt"] = 0; // This is if there is a barracks built on this tile
        conquerTileReasons[_otherEmpire]["FortBuilt"] = 0; // This is if there is a fort built on this tile
    }

    /*
    * The below will loop through all the reasons and update the diplomacy of all the empires
    * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
    * @retun int total This is all the added up reasons to conquer this tile
    */
    public int UpdateTileReasonsOfAllEmpires(EmpireClass _otherEmpire)
    {
        int total = 0;
        total += conquerTileReasons[_otherEmpire]["Boardering"];
        total += conquerTileReasons[_otherEmpire]["Garrison"];
        total += conquerTileReasons[_otherEmpire]["YourTroops"];
        total += conquerTileReasons[_otherEmpire]["TileReplenish"];
        total += conquerTileReasons[_otherEmpire]["Income"];
        total += conquerTileReasons[_otherEmpire]["EmpireConquer"];
        total += conquerTileReasons[_otherEmpire]["Attacked"];
        total += conquerTileReasons[_otherEmpire]["ImportantTile"];
        total += conquerTileReasons[_otherEmpire]["MineBuilt"];
        total += conquerTileReasons[_otherEmpire]["BarracksBuilt"];
        total += conquerTileReasons[_otherEmpire]["FortBuilt"];

        return total;
    }

    /*
    * The below function is used to update the value for a reason why the tile reason has increased or decreased.
    * @param string _reason This is the reason that the tile reason is increasing or decreasing
    * @param int _newValue This is the new value that it will be set to.
    * @param EmpireClass _otherEmpire This is the other empire that the tile reasons will be set up for
    */
    public void ChangeValueInTileReasons(string _reason, int _newValue, EmpireClass _otherEmpire)
    {
        conquerTileReasons[_otherEmpire][_reason] = _newValue;
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
            SetTroopPresent(UnityEngine.Random.Range(15, 25));
            SetTroopAdding(UnityEngine.Random.Range(3, 5));
            SetCurrentPopulation(UnityEngine.Random.Range(50, 75));
            SetAddingPopulation(UnityEngine.Random.Range(10, 15));
            SetIncome(UnityEngine.Random.Range(10, 15));
            SetAmeneties(UnityEngine.Random.Range(1, 3));

            int tileRandom = UnityEngine.Random.Range(1, 25);
            if (tileRandom <= 15)
            {
                thisTileType = TileType.None;
            }
            else if (tileRandom > 15 && tileRandom < 19)
            {
                thisTileType = TileType.Plain;
            }
            else
            {
                thisTileType = TileType.Mine;
            }
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
     * The below function is used when a new building has been built so that its effects can be updated on the tile
     * @Param string _buildingName This is the name of the building that is has been built
     */ 
    public void BuildingBuilt(string _buildingName)
    {
        if (_buildingName == "Mine")
        {
            SetIncome(GetIncome() + 30);
        }
        else if (_buildingName == "Barracks")
        {
            SetTroopAdding(GetTroopAdding() + 5);
        }
        else if (_buildingName == "Fort")
        {
            SetTroopPresent(GetTroopPresent() + 50);
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

    /*
     * The below function is used to set the population of this tile
     * @param float _newPopulation This is the new population
     */
    public void SetCurrentPopulation(float _newPopulation)
    {
        currentPopulation = (int)_newPopulation;
        if (corruptPopulation < 0)
        {
            corruptPopulation = 0;
        }
    }

    /*
     * The below function is used to set the adding population of a tile
     * @param int _newAddingPopulation This is the new adding population
     */ 
    public void SetAddingPopulation(int _newAddingPopulation)
    {
        addingPopulation = _newAddingPopulation;
    }

    /*
     * The below will return this tiles adding population
     * @return int addingPopulation This is current adding population number
     */ 
    public int GetAddingPopulation()
    {
        return addingPopulation;
    }

    /*
     * The below will return the current population
     * @return int currentPopulation This is the current population of this tile
     */ 
    public int GetCurrentPopulation()
    {
        return currentPopulation;
    }

    /*
     * The below function will be used to set the ameneties for this tile
     * @param int _newAmeneties This is the new amount of ameneties that is present on this tile
     */ 
    public void SetAmeneties(int _newAmeneties)
    {
        currentAmeneties = _newAmeneties;
    }

    /*
     * The below function will return the tiles current ameneties
     * @return int currentAmeneties This is the current ameneties for this tile
     */ 
    public int GetAmeneties()
    {
        return currentAmeneties;
    }

    /*
     * The below function is used to set the new corrupt population
     * @param float _newAmount This is the new number that corruptPopulation will be set to
     */
    public void SetCorruptPopulation(float _newAmount)
    {
        corruptPopulation = (int)_newAmount;
        if (corruptPopulation < 0)
        {
            corruptPopulation = 0;
        }

        if (corruptPopulation > currentPopulation / 5)
        {
            corruptPopulation = currentPopulation / 5;
        }
    }

    /*
     * The below function is used to get the corrupt population number
     * @return int corruptPopulation This is how much corrupt population is present in this tile.
     */ 
    public int GetCorruptPopulation()
    {
        return corruptPopulation;
    }
}
