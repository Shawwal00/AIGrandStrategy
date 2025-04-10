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
    public InternalModule InternalModule; // Handles everything occuring inside the empire

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private List<MapTile> allTilesWithBuildings; // This is a list of all the tiles which have a building

    private int empireNumber = 0;

    private Color empireColor = Color.white; // Is default to white but will be changed to another colour

    private bool populationMigrating = false;

    private void Awake()
    {
        // Lists
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();
        allTilesWithBuildings = new List<MapTile>();

        //Gameobjects
        GameManager = GameObject.Find("GameManager");

        //Scripts
        MapBoardScript = GameManager.GetComponent<MapBoard>();
        AiMain = GameManager.GetComponent<AIMain>();

        //Modules
        DiplomacyModule = this.AddComponent<DiplomacyModule>();
        WarModule = this.AddComponent<WarModule>();
        EconomyModule = this.AddComponent<EconomyModule>();
        InternalModule = this.AddComponent<InternalModule>();

        WarModule.SetThisEmpire(this);
        DiplomacyModule.SetThisEmpire(this);
        EconomyModule.SetThisEmpire(this);
        InternalModule.SetThisEmpire(this);
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
     * The below function will return the current safest tile that the empire has of a particular type
     * @param MapTile.TileType _type This is what type of tile you are searching for in particular
     * @param string _name This is the specific name of the type
     */
    public MapTile GetSafestTypeTile(MapTile.TileType _type, string _name)
    {
        UpdateOwnedTiles();
        MapTile safeTile = null;
        int tileDistance = 0;
        List<EmpireClass> allEmpires = WarModule.GetAllEmpiresInGame();
        List<EmpireClass> copyAllEmpires = new List<EmpireClass>();
        foreach (var empires in allEmpires)
        {
            copyAllEmpires.Add(empires);
        }
        copyAllEmpires.Remove(this);

        foreach (MapTile tile in ownedTiles)
        {
            if (tile.thisTileType == _type && tile.buildingData.GetBuildingDataOwned(_name) == 0)
            {
                if (safeTile == null)
                {
                    safeTile = tile;
                }
                else
                {
                    List<MapTile> tilesToCheck = new List<MapTile>();
                    List<MapTile> tilesToAdd = new List<MapTile>();
                    List<int> tileChecked = new List<int>();
                    List<MapTile> copyTileList = new List<MapTile>();
                    int currentDistance = 0;
                    foreach (var connectedTile in tile.GetAllConnectedTiles())
                    {
                        tilesToCheck.Add(connectedTile);
                    }
                    while (tilesToCheck.Count > 0)
                    {
                        foreach (var tileCheck in tilesToCheck)
                        {
                            currentDistance += 1;
                            foreach (var empire in copyAllEmpires)
                            {
                                if (tileCheck.GetOwner() == empire.GetEmpireNumber())
                                {
                                    if (currentDistance < tileDistance)
                                    {
                                        safeTile = tile;
                                        tileDistance = currentDistance;
                                    }
                                }
                            }
                            bool numberFound = false;
                            foreach (var number in tileChecked)
                            {
                                if (number == tileCheck.GetTileNumber())
                                {
                                    numberFound = true;
                                    break;
                                }
                            }
                            if (numberFound == false)
                            {
                                copyTileList.Add(tileCheck);
                            }
                        }

                        foreach (var copyTile in copyTileList)
                        {
                            tilesToAdd.Add(copyTile);
                            tilesToCheck.Remove(copyTile);
                            tileChecked.Add(copyTile.GetTileNumber());
                        }

                        copyTileList.Clear();

                        foreach (var tileAdd in tilesToAdd)
                        {
                            foreach (var tileAddConnection in tileAdd.GetAllConnectedTiles())
                            {
                                bool numberFound = false;
                                foreach (var number in tileChecked)
                                {
                                    if (number == tileAddConnection.GetTileNumber())
                                    {
                                        numberFound = true;
                                        break;
                                    }
                                }
                                if (numberFound == false)
                                {
                                    copyTileList.Add(tileAddConnection);
                                }
                            }
                        }
                    }
                }
            }
        }
        return safeTile;
    }

    /*
     * The below function will get the tiles that boarder another empire
     */
    public MapTile GetBoarderTilesWithThreateningEmpire()
    {
        
        // Maybe change this so that it will protect tiles around important buildings first
        int amonuntOfTilesProtected = 0;
        int highestAmountOfTilesProtected = 0;
        MapTile bestTile = null;

        foreach (var tile in allTilesWithBuildings)
        {
            //Get the one with the most protection around it
            foreach (var connections in tile.GetAllConnectedTiles())
            {
                if (connections.thisTileType == MapTile.TileType.Plain && connections.buildingData.GetBuildingDataOwned("Fort") == 0)
                {
                    amonuntOfTilesProtected += 1;
                }
            }

            if (bestTile == null)
            {
                bestTile = tile;
                highestAmountOfTilesProtected = amonuntOfTilesProtected;
            }
            else
            {
                if (amonuntOfTilesProtected > highestAmountOfTilesProtected)
                {
                    bestTile = tile;
                    highestAmountOfTilesProtected = amonuntOfTilesProtected;
                }
            }
        }

        if (bestTile != null)
        {
            foreach (var tile in bestTile.GetAllConnectedTiles())
            {
                if (tile.buildingData.GetBuildingDataOwned("Fort") == 0)
                {
                    return tile;
                }
            }
        }

        // If not protecting high value tile then protect boarder against an empire
        UpdateOwnedTiles();
        List<EmpireClass> threateningEmpires = DiplomacyModule.GetDislikedEmpires();
        List<MapTile> allBoarderingTiles = new List<MapTile>();
        foreach (MapTile tile in ownedTiles)
        {
            if (tile.thisTileType == MapTile.TileType.Plain)
            {
                foreach (var tileBoarder in tile.GetAllConnectedTiles())
                {
                    foreach (var empire in threateningEmpires)
                    {
                        if (tileBoarder.GetOwner() == empire.GetEmpireNumber())
                        {
                            if (!(allBoarderingTiles.Contains(tile)))
                            {
                                allBoarderingTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        MapTile highestTile = null;
        int highestValue = 0;
        foreach (MapTile tile in allBoarderingTiles)
        {
            if (tile.buildingData.GetBuildingDataOwned("Fort") == 0)
            {
                tile.ChangeValueInBuildFort("Garrison", tile.GetTroopPresent(), this);
                tile.ChangeValueInBuildFort("TileReplenish", tile.GetTroopAdding(), this);
                tile.ChangeValueInBuildFort("Income", tile.GetIncome(), this);
                if (highestTile == null)
                {
                    highestTile = tile;
                    highestValue = tile.UpdateBuildFortForAllTiles(this);
                }
                else
                {
                    if (highestValue < tile.UpdateBuildFortForAllTiles(this))
                    {
                        highestTile = tile;
                        highestValue = tile.UpdateBuildFortForAllTiles(this);
                    }
                }
            }
        }
        

        return highestTile;
    }

    /*
     * The below function is used to get the tiles that are at the edge of this empire 
     */
    public List<MapTile> GetTileAtEdge()
    {
        List<EmpireClass> atWarEmpires = WarModule.GetAtWarEmpires();
        List<MapTile> allBoarderingTiles = new List<MapTile>();
        foreach (MapTile tile in ownedTiles)
        {
            foreach (var tileBoarder in tile.GetAllConnectedTiles())
            {
                if (tileBoarder.GetOwner() != GetEmpireNumber())
                {
                    if (!(allBoarderingTiles.Contains(tile)))
                    {
                        allBoarderingTiles.Add(tile);
                    }
                    break;
                }
            }
        }
        return allBoarderingTiles;
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
     * The below function is used to migrate the population of an empire to another empire
     */
    public void MigratePopulation()
    {
        // Migrate to boardering empires
        // Migrate 0.01 % of your owned tiles to each other empire assuming that they have enough ameneties - split between all the tiles - so split between 0.3%

        //Migrate if no ameneties

        if (populationMigrating == true)
        {
            List<EmpireClass> happyEmpires = new List<EmpireClass>();
            foreach (var empire in WarModule.GetBoarderingEmpires())
            {
                if (empire.GetPopulationMigrating() == false)
                {
                    happyEmpires.Add(empire);
                }
            }

            float totalPopulationChange = 0;
            foreach (var tile in ownedTiles)
            {
                totalPopulationChange += tile.GetCurrentPopulation() - (tile.GetCurrentPopulation() * 0.97f);
                tile.SetCurrentPopulation(tile.GetCurrentPopulation() * 0.97f);
            }


            if (happyEmpires.Count > 0)
            {
                float perEmpire = totalPopulationChange / happyEmpires.Count;
                foreach (var empire in happyEmpires)
                {
                    float addPerTile = perEmpire / empire.ownedTiles.Count;
                    foreach (var tile in empire.ownedTiles)
                    {
                        tile.SetCurrentPopulation(tile.GetCurrentPopulation() + addPerTile);
                    }
                }
            }
            else
            {
                // If no other empire then convert to corrupt population
                foreach (var tile in ownedTiles)
                {
                    float addPerTile = totalPopulationChange / ownedTiles.Count;
                    tile.SetCorruptPopulation(tile.GetCorruptPopulation() + addPerTile/2);
                }
            }
        }
    }

    /*
     * Used to get the total population of the empire
     * @return int total This is the total amount of population inside an empire
     */ 
    public int GetAllPopulation()
    {
        UpdateOwnedTiles();
        int total = 0;
        foreach (var tile in ownedTiles)
        {
            total += tile.GetCurrentPopulation();
        }
        return total;
    }

    /*
   * Used to get the total corrupt population of the empire
   * @return int total This is the total amount of corrupt population inside an empire
   */
    public int GetAllCorruptPopulation()
    {
        UpdateOwnedTiles();
        int total = 0;
        foreach (var tile in ownedTiles)
        {
            total += tile.GetCorruptPopulation();
        }
        return total;
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

    /*
     * This will a list of all the tiles with special buildings on them
     * @retun List<MapTile> allTilesWithBuildings This is a list of all tiles which have special buildings on them
     */
    public List<MapTile> GetAllTilesWithSpecialBuildings()
    {
        return allTilesWithBuildings;
    }

    /*
     * The below function will set the population migrating to a new value
     * @param bool _newValue This is the new value for if a population is migrating or not.
     */
    public void SetPopulationMigrating(bool _newValue)
    {
        populationMigrating = _newValue;
    }

    /*
     * The below function will get the value of populationMigrating
     * @return bool populationMigrating This is if the population is migrating or not
     */ 
    public bool GetPopulationMigrating()
    {
        return populationMigrating; 
    }
}
