using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
 * This script is responsible for the war aspect of an empire.
*/


public class WarModule : MonoBehaviour
{
    private GameObject GameManager;
    private SetUpEmpires SetUpEmpires; // This script sets up the inital empires

    private EmpireClass thisEmpire;
    private List<EmpireClass> boarderingEmpires;
    private List<EmpireClass> atWarEmpires;
    private List<EmpireClass> empiresDefeatedInBattle;
    private List<EmpireClass> allEmpiresInGame;
    private Dictionary<EmpireClass, int> threatRatings = new Dictionary<EmpireClass, int>(); 
    private Dictionary<EmpireClass, Dictionary<string, int>> threatRatingReasons = new Dictionary<EmpireClass, Dictionary<string, int>>(); // These are the reasons that an empire feels threatened by another empire.

    //Army Movements
    private Dictionary<MapTile, Dictionary<MapTile, MapTile>> tileMovementPathfinding = new Dictionary<MapTile, Dictionary<MapTile, MapTile>>();
    private Dictionary<MapTile, MapTile> tileMovementRefrecnes = new Dictionary<MapTile, MapTile>();
    private Dictionary<MapTile, int> tileTroopMovement = new Dictionary<MapTile, int>();
    private List<MapTile> tileMovementList = new List<MapTile>();

    private int troopNumber = 0; 
    private float updateTroopNumberTime = 0;
    private int troopReplenishAmount = 0;

    public bool trainTroops = true;

    private int warDiplomacyNumber = -25; // This is the number at which a AI will go to war with another Empire
    private int threatValue = 20; //This is the number at which the AI will consider another empire to be a threat
    private int conquerTileValue = -50; //This is the value the tile has to be at least for this empire to conquer it
    private float surroundingTileValue = 0.05f; //The value that increases morale for surrounding tiles

    //Threat reason values
    private int rThreatBoardering = 40;
    private int rTroops = 50;
    private int rIncome = 10;
    private int rTotalMoney = 10;
    private int rReplenishRate = 15;
    private int rGarrison = 15;
    private int rMorePopulation = 5;
    private int rMoreCorruptPopulation = 5;

    //Conquer Tile reason values
    private int rTileBoardering = 30;
    private int rYourTroops = 60;
    private int rOtherEmpireConquer = 20;
    private int rAttacked = 0;
    private int rMineTile = 40;
    private int rPlainTile = 40;
    private int rFortBuilt = -40;
    private int rMineBuilt = 40;
    private int rBarracksBuilt = 40;
    private int rDividePopulationValue = 50;
    private int rDivideCorruptPopulationValue = 10;
    private int rSurroundingTileSingle = 40;
    private int rOverwhelmingTroops = 100;

    //Move To tile reasons
    private int rImportantTile = 40;
    private int rEnoughTroopsWar = 60;
    private int rNeutralTroops = 20;
    private int rDislikedEmpire = 10;
    private int mDistance = 3;
    private int rWarEmpire = 60;

    private float delay = 0.0f; // This is the speed of the game

    private void Awake()
    {
        //Gameobject
        GameManager = GameObject.Find("GameManager");

        //Scripts
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();

        //Lists
        boarderingEmpires = new List<EmpireClass>();
        atWarEmpires = new List<EmpireClass>();
        allEmpiresInGame = new List<EmpireClass>();
    }

    /*
     * The below function sets a new value for the warDiplomacyValue
     * @param int _newValue The new value assigned to the warDiplomacyNumber
     */ 
    public void SetWarDiplomacyValue(int _newValue)
    {
        warDiplomacyNumber = _newValue;
    }

    /*
    * The below function sets a new value for the threatValue
    * @param int _newValue The new value assigned to the threatValue
    */
    public void SetThreatValue(int _newValue)
    {
        threatValue = _newValue;
    }

    /*
    * The below function sets a new value for the conquerValue
    * @param int _newValue The new value assigned to the conquerValue
    */
    public void SetConquerTileValue(int _newValue)
    {
        conquerTileValue = _newValue;
    }

    /*
     * This function is used when all the other empires have been set up and the inital relationships should also be set up
     * @param List<EmpireClass> _allEmpires This is a list of all the empires within the game.
     */
    public void MeetingAllEmpires(List<EmpireClass> _allEmpires)
    {
        for (int i = 0; i < _allEmpires.Count; i++)
        {
            if (_allEmpires[i] != thisEmpire)
            {
                thisEmpire.DiplomacyModule.MetEmpire(_allEmpires[i]);
                MetEmpire(_allEmpires[i]);
                UpdateThreatRatingsOfAllEmpires();
            }
        }

        foreach (var empire in _allEmpires)
        {
            if (empire != thisEmpire)
            {
                allEmpiresInGame.Add(empire); //Adds every empire except your own
            }
        }
    }

    /*
     * The below function will be used to update the threat rating parameters every turn - these will be the ones that are not dependent on 
     * other variables.
     */ 
    public void UpdateThreatReasons()
    {
        foreach (EmpireClass _empireClass in allEmpiresInGame)
        {
            if (_empireClass.WarModule.GetTroopNumber() > GetTroopNumber() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "Troops", rTroops * 2);
            }
            else if (_empireClass.WarModule.GetTroopNumber() > GetTroopNumber())
            {
                ChangeValueInThreatRatings(_empireClass, "Troops", rTroops);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "Troops", -rTroops);
            }

            if (_empireClass.EconomyModule.GetIncomeValue() > thisEmpire.EconomyModule.GetIncomeValue() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "Income", rIncome * 2);
            }
            else if (_empireClass.EconomyModule.GetIncomeValue() > thisEmpire.EconomyModule.GetIncomeValue())
            {
                ChangeValueInThreatRatings(_empireClass, "Income", rIncome);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "Income", -rIncome);
            }

            if (_empireClass.EconomyModule.GetCurrentMoney() > thisEmpire.EconomyModule.GetCurrentMoney() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "TotalMoney", rTotalMoney * 2);
            }
            else if (_empireClass.EconomyModule.GetCurrentMoney() > thisEmpire.EconomyModule.GetCurrentMoney())
            {
                ChangeValueInThreatRatings(_empireClass, "TotalMoney", rTotalMoney);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "TotalMoney", -rTotalMoney);
            }

            if (_empireClass.WarModule.GetReplinishAmount() > troopReplenishAmount * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "ReplenishRate", rReplenishRate * 2);
            }
            else if (_empireClass.WarModule.GetReplinishAmount() > troopReplenishAmount)
            {
                ChangeValueInThreatRatings(_empireClass, "ReplenishRate", rReplenishRate);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "ReplenishRate", -rReplenishRate);
            }

            if (_empireClass.WarModule.GetAllGarrisons() > GetAllGarrisons() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "AllGarrisons", rGarrison * 2);
            }
            else if (_empireClass.WarModule.GetAllGarrisons() > GetAllGarrisons())
            {
                ChangeValueInThreatRatings(_empireClass, "AllGarrisons", rGarrison);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "AllGarrisons", -rGarrison);
            }

            if (_empireClass.GetAllPopulation() > thisEmpire.GetAllPopulation() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "Population", rMorePopulation * 2);
            }
            else if (_empireClass.GetAllPopulation() > thisEmpire.GetAllPopulation())
            {
                ChangeValueInThreatRatings(_empireClass, "Population", rMorePopulation);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "Population", -rMorePopulation);
            }

            if (_empireClass.GetAllCorruptPopulation() > thisEmpire.GetAllCorruptPopulation() * 2)
            {
                ChangeValueInThreatRatings(_empireClass, "Population", rMoreCorruptPopulation * 2);
            }
            else if (_empireClass.GetAllCorruptPopulation() > thisEmpire.GetAllCorruptPopulation())
            {
                ChangeValueInThreatRatings(_empireClass, "Population", rMoreCorruptPopulation);
            }
            else
            {
                ChangeValueInThreatRatings(_empireClass, "Population", -rMoreCorruptPopulation);
            }
        }
        thisEmpire.FunctionFinished();
    }

    /*
    * The below function will occur when another empire has been met for the first time.
    * @param EmpireClass _otherEmpire This is the new empire you have just met
    */
    private void MetEmpire(EmpireClass _otherEmpire)
    {
        threatRatings[_otherEmpire] = 0;
        threatRatingReasons[_otherEmpire] = new Dictionary<string, int>();
        threatRatingReasons[_otherEmpire]["Boardering"] = 0; // This is if the empire is boardering
        threatRatingReasons[_otherEmpire]["Troops"] = 0; // This is if the other empire has more troops then you
        threatRatingReasons[_otherEmpire]["Income"] = 0; // This is if they have a higher monthly income
        threatRatingReasons[_otherEmpire]["TotalMoney"] = 0; // This is the total amount of money that they have
        threatRatingReasons[_otherEmpire]["ReplenishRate"] = 0; // This is how fast there troops would replenish with all thier lands
        threatRatingReasons[_otherEmpire]["AllGarrisons"] = 0; // This is how much all thier garrisons add up to.
        threatRatingReasons[_otherEmpire]["Population"] = 0; // This is if the other empire has a higher population then you.
        threatRatingReasons[_otherEmpire]["CorruptPopulation"] = 0; // This is if the other empire has a higher corrupt population then you.
    }

    /*
    * The below will loop through all the reasons and update the diplomacy of all the empires
    */
   private void UpdateThreatRatingsOfAllEmpires()
    {
        for (int i = 0; i < GetBoarderingEmpires().Count; i++)
        {
            int total = 0;
            EmpireClass otherEmpire = thisEmpire.WarModule.GetBoarderingEmpires()[i];

            total += threatRatingReasons[otherEmpire]["Boardering"];
            total += threatRatingReasons[otherEmpire]["Troops"];
            total += threatRatingReasons[otherEmpire]["Income"];
            total += threatRatingReasons[otherEmpire]["TotalMoney"];
            total += threatRatingReasons[otherEmpire]["ReplenishRate"];
            total += threatRatingReasons[otherEmpire]["AllGarrisons"];
            total += threatRatingReasons[otherEmpire]["Population"];
            total += threatRatingReasons[otherEmpire]["CorruptPopulation"];

            threatRatings[otherEmpire] = total;
        }
    }

    /*
    * The below function is used to update the value for a reason why the threat rating has increased or decreased.
    * @param EmpireClass _otherEmpire This is the other empire
    * @param string _reason This is the reason that the threat rating is increasing or decreasing
    * @param int _newValue This is the new value that it will be set to.
    */
    private void ChangeValueInThreatRatings(EmpireClass _otherEmpire, string _reason, int _newValue)
    {
        threatRatingReasons[_otherEmpire][_reason] = _newValue;
        UpdateThreatRatingsOfAllEmpires();
    }

    /*
     * This is so the script knows which empire it is.
     * @param EmpireClass _thisEmpire This is a refrence to the this empire
     */
    public void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;
    }

    /*
     * The below function is used to update the replinish amount of the empire whenever a new tile is conquered or building constructed
     */ 
    public void UpdateReplinishAmount()
    {
        for (int i = 0; i < thisEmpire.GetOwnedTiles().Count; i++)
        {
             troopReplenishAmount += thisEmpire.GetOwnedTiles()[i].GetTroopAdding();
        }
    }

    /*
     * This updates all the tiles with thier new troops.
     */
    public void UpdateAllTroopCount()
    {
      int totalTroops = 0;
        if (thisEmpire.GetOwnedTiles().Count > 0)
        {
            foreach (var tile in thisEmpire.GetOwnedTiles())
            {
                if (thisEmpire.EconomyModule.GetTrainTroops())
                {
                    tile.SetTroopPresent(tile.GetTroopPresent() + (tile.GetCurrentPopulation() / 50) - (tile.GetCorruptPopulation() / 40) + tile.GetTroopAdding());
                }
                totalTroops += tile.GetTroopPresent();
            }
        }
        SetTroopNumber(totalTroops);
        thisEmpire.FunctionFinished();
    }

    /*
     * The below function is used to move the troops around the empire and then to attack any tiles.
     */
    public IEnumerator ConquerTerritory()
    {
        thisEmpire.UpdateOwnedTiles();
        foreach (var ownedTile in thisEmpire.GetOwnedTiles())
        {
            if (ownedTile.GetTroopPresent() > 10) // For performance
            {
                int tilesSafeCount = 0;
                int totalOpposingTroops = 0;
                bool tooManyTroops = false;

                // Check to see if the troops on the tile should be moved or not
                foreach (var expandingConnection in ownedTile.GetAllConnectedTiles())
                {
                    bool alliedTrue = false;
                    bool owned = false;
                    if (thisEmpire.DiplomacyModule.GetAlliedEmpires().Count != allEmpiresInGame.Count - 1)
                    {
                        foreach (var alliedEmpire in thisEmpire.DiplomacyModule.GetAlliedEmpires())
                        {
                            if (expandingConnection.GetOwner() == alliedEmpire.GetEmpireNumber())
                            {
                                alliedTrue = true;
                            }
                        }
                    }

                    if (expandingConnection.GetOwner() != thisEmpire.GetEmpireNumber() && alliedTrue == false)
                    {
                        totalOpposingTroops += expandingConnection.GetTroopPresent();
                    }

                    if (atWarEmpires.Count > 0)
                    {
                        foreach (var empire in atWarEmpires)
                        {
                            if (expandingConnection.GetOwner() == empire.GetEmpireNumber())
                            {
                                owned = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (expandingConnection.GetOwner() != thisEmpire.GetEmpireNumber())
                        {
                            owned = true;
                        }
                    }

                    if (alliedTrue == true || owned == false)
                    {
                        tilesSafeCount += 1;
                    }
                }

                // Do a check to see if too many troops are on the tile if so then move only some of the troops
                int troopsToMove = ownedTile.GetTroopPresent() - 1;
                int troopsToLeaveBehind = 1;
                if (ownedTile.GetAllConnectedTiles().Count != tilesSafeCount)
                {
                    if (ownedTile.GetTroopPresent() - totalOpposingTroops > 10)
                    {
                        tooManyTroops = true;
                        troopsToMove = ownedTile.GetTroopPresent() - totalOpposingTroops - 1;
                        troopsToLeaveBehind = totalOpposingTroops;
                    }
                }

                if (ownedTile.GetAllConnectedTiles().Count == tilesSafeCount || tooManyTroops == true) // Move troops all connections to this tile are safe
                {
                    //Check all tile edges 
                    //Check tile reasons on why to move to this tile
                    // Pick the tile with the highest reason

                    List<MapTile> tileEdges = thisEmpire.GetTileAtEdge();
                    if (tileEdges.Count == 0)
                    {
                        break;
                    }
                    List<MapTile> ignoreTiles = new List<MapTile>();
                    Dictionary<MapTile, MapTile> tileCameFrom = new Dictionary<MapTile, MapTile>();
                    bool foundTile = false;
                    MapTile highestReason = null;
                    while (foundTile == false)
                    {
                        int reasonNumber = 0;
                        foreach (var tile in tileEdges)
                        {
                            if (!(ignoreTiles.Contains(highestReason)))
                            {
                                if (highestReason == null)
                                {
                                    highestReason = tile;
                                    UpdateTileMoveReasons(highestReason, ownedTile);
                                    reasonNumber = tile.UpdateTileMoveForAllTile(thisEmpire);
                                }
                                else
                                {
                                    UpdateTileMoveReasons(tile, ownedTile);
                                    if (reasonNumber < tile.UpdateTileMoveForAllTile(thisEmpire))
                                    {
                                        highestReason = tile;
                                        reasonNumber = tile.UpdateTileMoveForAllTile(thisEmpire);
                                    }
                                }
                            }
                        }
                        //Pathfinding check
                        List<MapTile> reachedTiles = null;

                        tileCameFrom = GreedysBestFirstSearch(ownedTile, highestReason, out reachedTiles);

                        if (reachedTiles.Contains(highestReason))
                        {
                            //Check if the unit can reach there if not then add to ignore list.
                            foundTile = true;
                        }
                        else
                        {
                            ignoreTiles.Add(highestReason);
                        }

                        if (tileEdges.Count == ignoreTiles.Count) // Tile is compleatly surrounded by other Empires
                        {
                            break;
                        }

                    }

                    // Make a list of all the important information then do the movement after
                    if (foundTile == true)
                    {
                        tileMovementList.Add(ownedTile);
                        tileMovementRefrecnes[ownedTile] = highestReason;
                        tileTroopMovement[ownedTile] = troopsToMove;
                        tileMovementPathfinding[ownedTile] = tileCameFrom;
                    }
                }
            }
        }

        //Movement occurs below outside the for loop so that troops only move once and not multiple times
        foreach (MapTile tile in tileMovementList)
        {
            List<MapTile> path = new List<MapTile>();
            MapTile currentPathTile = null;
            path.Add(tileMovementRefrecnes[tile]);
            bool tileFound = false;
            currentPathTile = tileMovementRefrecnes[tile];
            while (tileFound == false)
            {
                currentPathTile = tileMovementPathfinding[tile][currentPathTile];
                if (currentPathTile == null)
                {
                    tileFound = true;
                }
                else
                {
                    path.Add(currentPathTile);
                }

            }
            MapTile tileToGoTo = path[path.Count - 2];
            tile.GetComponent<MeshRenderer>().material.color = Color.yellow;
            tileToGoTo.GetComponent<MeshRenderer>().material.color = Color.grey;
            yield return new WaitForSeconds(delay);
            tileToGoTo.SetTroopPresent(tileToGoTo.GetTroopPresent() + tileTroopMovement[tile]);
            tile.SetTroopPresent(tile.GetTroopPresent()  - tileTroopMovement[tile]);


            tile.SetOwner(tile.GetOwner());
            tileToGoTo.SetOwner(tileToGoTo.GetOwner());
        }
        tileMovementList.Clear();
        tileMovementPathfinding.Clear();
        tileTroopMovement.Clear();
        tileMovementRefrecnes.Clear();

        thisEmpire.UpdateExpandingTilesOfTile();
        bool alliedTile = false;
        // Loop through all your owned tiles
        // for each of those geet expanding tiles 
        // If expanding greater then - then do reason check to see if you should conquer the tile

        foreach (var ownedTile in thisEmpire.GetOwnedTiles())
        {
            MapTile lowestTile = null;
            int tileReasonValue = 0;
            foreach (var expandingConnection in ownedTile.GetAllConnectedTiles())
            {
                alliedTile = false;
                if (expandingConnection.GetOwner() != thisEmpire.GetEmpireNumber())
                {
                    foreach (EmpireClass alliedEmpire in thisEmpire.DiplomacyModule.GetAlliedEmpires())
                    {
                        if (alliedEmpire.GetEmpireNumber() == expandingConnection.GetOwner())
                        {
                            alliedTile = true;
                            break;
                        }
                    }
                    if (alliedTile == false)
                    {
                        bool warEmpire = false;
                        foreach (var empire in atWarEmpires)
                        {
                            if (expandingConnection.GetOwner() == empire.GetEmpireNumber())
                            {
                                warEmpire = true;
                                break;
                            }
                        }
                        if (lowestTile == null)
                        {
                            if (expandingConnection.GetOwner() == 0 || warEmpire == true)
                            {
                                CheckTileForReasons(expandingConnection, ownedTile, false);
                                lowestTile = expandingConnection;
                                tileReasonValue = expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire);
                            }
                        }
                        else
                        {
                            // The below is inspired by Monte Carlo Tree Search - the code witll look at 3 tiles in advance and go for the tile with the highest values averaged
                            if (expandingConnection.GetOwner() == 0 || warEmpire == true)
                            {
                                CheckTileForReasons(expandingConnection, ownedTile, false);

                                if (expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire) > tileReasonValue)
                                {
                                    tileReasonValue = expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire);
                                    lowestTile = expandingConnection;
                                }
                                ;
                                foreach (var secondTile in expandingConnection.GetAllConnectedTiles())
                                {
                                    if (secondTile.GetOwner() != thisEmpire.GetEmpireNumber())
                                    {
                                        CheckTileForReasons(secondTile, ownedTile, false);
                                        int secondAverage = (secondTile.UpdateTileReasonsOfAllEmpires(thisEmpire) + expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire)) / 2;

                                        if (secondAverage > tileReasonValue)
                                        {
                                            tileReasonValue = expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire);
                                            lowestTile = expandingConnection;
                                        }
                                    }
                                    foreach (var thirdTile in secondTile.GetAllConnectedTiles())
                                    {
                                        if (thirdTile.GetOwner() != thisEmpire.GetEmpireNumber())
                                        {
                                            CheckTileForReasons(thirdTile, ownedTile, false);
                                            int thirdAverage = (thirdTile.UpdateTileReasonsOfAllEmpires(thisEmpire) + secondTile.UpdateTileReasonsOfAllEmpires(thisEmpire) + expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire)) / 3;

                                            if (thirdAverage > tileReasonValue)
                                            {
                                                tileReasonValue = expandingConnection.UpdateTileReasonsOfAllEmpires(thisEmpire);
                                                lowestTile = expandingConnection;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (lowestTile != null && lowestTile.GetTroopPresent() * lowestTile.GetTileDefensiveBonus() * lowestTile.GetSurroundingTileBonus() < ownedTile.GetTroopPresent()  && tileReasonValue > conquerTileValue)
            {
                //Conquer the tile
                ownedTile.GetComponent<MeshRenderer>().material.color = Color.cyan;
                lowestTile.GetComponent<MeshRenderer>().material.color = Color.magenta;
                yield return new WaitForSeconds(delay);
                lowestTile.SetOwner(thisEmpire.GetEmpireNumber());

                //Do check in here for many troops you want to remian behind
                int leavingTileTroops = 0;
                int goingToTileTroops = 0;
                foreach (var tile in ownedTile.GetAllConnectedTiles())
                {
                    if (tile.GetOwner() != thisEmpire.GetEmpireNumber())
                    {
                        leavingTileTroops += tile.GetTroopPresent();
                    }
                }

                foreach (var tile in lowestTile.GetAllConnectedTiles())
                {
                    if (tile.GetOwner() != thisEmpire.GetEmpireNumber())
                    {
                        goingToTileTroops += tile.GetTroopPresent();
                    }
                }

                float totalTroops = leavingTileTroops + goingToTileTroops;
                float availableTroops = ownedTile.GetTroopPresent() - lowestTile.GetTroopPresent();
                if (totalTroops > availableTroops)
                {
                    float troopMultiplier = availableTroops / totalTroops;
                    lowestTile.SetTroopPresent((int)(goingToTileTroops * troopMultiplier));
                    ownedTile.SetTroopPresent((int)(leavingTileTroops * troopMultiplier));
                }
                else
                {
                    lowestTile.SetTroopPresent((int)(availableTroops - leavingTileTroops));
                    ownedTile.SetTroopPresent((int)(leavingTileTroops));
                }
           
                lowestTile.SetCurrentPopulation(lowestTile.GetCurrentPopulation() * 0.9f);
                lowestTile.SetCorruptPopulation(lowestTile.GetCorruptPopulation() * 1.1f);
                thisEmpire.EconomyModule.CalculateMoneyUpdateAmount();
                UpdateReplinishAmount();

                SetTroopNumber(GetTroopNumber() - (int)(lowestTile.GetTroopPresent() * lowestTile.GetTileDefensiveBonus() * lowestTile.GetSurroundingTileBonus()));

                foreach (var empire in allEmpiresInGame)
                {
                    CheckAllEmpireBoarders();
                }

                ownedTile.SetOwner(ownedTile.GetOwner());
                lowestTile.SetOwner(lowestTile.GetOwner());
            }
        }

        thisEmpire.FunctionFinished();
    }

    /*
     * The below is the pathfinding which is used o move units - Currently it is BFS only
     * @param MapTile _currentTile This is the current tile the unit is on
     * @param MapTile _destinationTile This is the tile that the unit will move towards
     * @return Dictionary<MapTile, MapTile> tileCameFrom This is the order of tiles that the unit should move to
     */
    public Dictionary<MapTile, MapTile> GreedysBestFirstSearch(MapTile _currentTile, MapTile _destinationTile, out List<MapTile> reached)
    {
        List<MapTile> frontier = new List<MapTile>();
        reached = new List<MapTile>();
        Dictionary<MapTile, MapTile> tileCameFrom = new Dictionary<MapTile, MapTile>();
        MapTile currentTile = null;

        frontier.Add(_currentTile);
        tileCameFrom.Add(_currentTile, null);

        //While will check every tile
        while (frontier.Count > 0)
        {
            currentTile = frontier[0];

            foreach (var tile in currentTile.GetAllConnectedTiles()) 
            {
                //Will not add if already in reached or frontier or the tile is not owned by you
                if (!reached.Contains(tile) && !frontier.Contains(tile) && tile.GetOwner() == thisEmpire.GetEmpireNumber())
                {
                    if (_destinationTile.transform.position.magnitude - tile.transform.position.magnitude < 
                        _destinationTile.transform.position.magnitude - tile.transform.position.magnitude)
                    {
                        frontier.Insert(0, tile);
                        tileCameFrom.Add(tile, currentTile);
                    }
                    else
                    {
                        frontier.Add(tile);
                        tileCameFrom.Add(tile, currentTile);
                    }
                }
            }
            reached.Add(currentTile);
            frontier.Remove(currentTile);

            if (reached.Contains(_destinationTile))
            {
                break;
            }
        }
        reached.Remove(reached[0]); // Remove the starting tile

        return tileCameFrom;
    }

    /*
     * Will update the reasons for moving to a specific tile
     * @param MapTile _tile This is the tile that you are using
     */
    public void UpdateTileMoveReasons(MapTile _tile, MapTile _currentTile)
    {
        Dictionary<EmpireClass, int> totalTroops = new Dictionary<EmpireClass, int>();
        foreach (var empire in atWarEmpires)
        {
            totalTroops[empire] = 0;
        }

        int totalNeutralTroops = 0;
        int totalEnemyTroops = 0;
        bool dislikedEmpire = false;
        bool warEmpire = false;
        foreach (var connectTile in _tile.GetAllConnectedTiles())
        {
            if (connectTile.thisTileType != MapTile.TileType.Plain)
            {
                _tile.ChangeValueInTileMove("ImportantConnectingTile", rImportantTile, thisEmpire);
            }
            foreach (var empire in atWarEmpires)
            {
                if (connectTile.GetOwner() == empire.GetEmpireNumber())
                {
                    totalTroops[empire] += totalTroops[empire];
                    warEmpire = true;
                }
            }
            if (connectTile.buildingData.GetBuildingDataOwned("Mine") == 1)
            {
                connectTile.ChangeValueInTileMove("MineBuilt", rMineBuilt, thisEmpire);
            }

            if (connectTile.buildingData.GetBuildingDataOwned("Barracks") == 1)
            {
                connectTile.ChangeValueInTileMove("BarracksBuilt", rBarracksBuilt, thisEmpire);
            }

            if (connectTile.buildingData.GetBuildingDataOwned("Fort") == 1)
            {
                connectTile.ChangeValueInTileMove("FortBuilt", rFortBuilt, thisEmpire);
            }
            if (connectTile.GetOwner() == 0)
            {
                totalNeutralTroops += connectTile.GetTroopPresent();
            }

            foreach (var empire in thisEmpire.DiplomacyModule.GetDislikedEmpires())
            {
                if (connectTile.GetOwner() == empire.GetEmpireNumber())
                {
                    dislikedEmpire = true;
                    break;
                }
            }

            bool alliedTile = false;
            foreach (var alliedEmpire in thisEmpire.DiplomacyModule.GetAlliedEmpires())
            {
                if (alliedEmpire.GetEmpireNumber() == connectTile.GetOwner())
                {
                    alliedTile = true;
                    break;
                }
            }

            if (connectTile.GetOwner() != thisEmpire.GetEmpireNumber() && alliedTile == false)
            {
                totalEnemyTroops += connectTile.GetTroopPresent();
            }
        }
        if (dislikedEmpire == true)
        {
            _tile.ChangeValueInTileMove("LikelyWar", rDislikedEmpire, thisEmpire);
        }

        if (warEmpire == true)
        {
            _tile.ChangeValueInTileMove("War", rWarEmpire, thisEmpire);
        }

        if (totalNeutralTroops > _tile.GetTroopPresent() * _tile.GetTileDefensiveBonus() * _tile.GetSurroundingTileBonus())
        {
            _tile.ChangeValueInTileMove("EnoughTroopsDefault", rNeutralTroops, thisEmpire);
        }

        int neededTroops = 0;
        if (totalEnemyTroops > _tile.GetTroopPresent() * _tile.GetTileDefensiveBonus() * _tile.GetSurroundingTileBonus())
        {
            neededTroops = totalEnemyTroops - _tile.GetTroopPresent();
            _tile.ChangeValueInTileMove("TroopDiffrence", neededTroops/10, thisEmpire);
        }
        else
        {
            _tile.ChangeValueInTileMove("EnoughTroopsWar", -rEnoughTroopsWar, thisEmpire);
        }

        List<MapTile> reachedTiles = null;
        GreedysBestFirstSearch(_currentTile, _tile, out reachedTiles);

        int rDistance = reachedTiles.Count * mDistance;
        if (rDistance > neededTroops)
        {
            rDistance = neededTroops - 10;
        }
        _tile.ChangeValueInTileMove("Distance", -rDistance, thisEmpire);
    }

    /*
     * The below check all the tiles of an empire to see if they are still boardering or currently boardering an empire
     */
    public void CheckAllEmpireBoarders()
    {
        thisEmpire.UpdateExpandingTilesOfTile();
        List<EmpireClass> foundEmpires = new List<EmpireClass>();
        List<EmpireClass> notFoundEmpires = new List<EmpireClass>();

        foreach (var empire in allEmpiresInGame)
        {
            notFoundEmpires.Add(empire);
        }

        foreach (MapTile tile in thisEmpire.GetExpandingTiles())
        {
            if (tile.GetOwner() != 0 && tile.GetOwner() != thisEmpire.GetEmpireNumber())
            {
                foreach (var empire in allEmpiresInGame)
                {
                    if (empire.GetEmpireNumber() == tile.GetOwner())
                    {
                        if (!foundEmpires.Contains(empire))
                        {
                            foundEmpires.Add(empire);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var foundEmpire in foundEmpires)
        {
            if (!boarderingEmpires.Contains(foundEmpire))
            {
                boarderingEmpires.Add(foundEmpire);

                ChangeValueInThreatRatings(foundEmpire, "Boardering", rThreatBoardering);
                foundEmpire.WarModule.ChangeValueInThreatRatings(thisEmpire, "Boardering", rThreatBoardering);

                // Set up the diplomacy of the other empire
                thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(foundEmpire, "Boardering", -thisEmpire.DiplomacyModule.GetRBoarderingValue());
                foundEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(thisEmpire, "Boardering", -thisEmpire.DiplomacyModule.GetRBoarderingValue());
            }
            notFoundEmpires.Remove(foundEmpire);
        }

        foreach (var notFoundEmpire in notFoundEmpires)
        {
            if (boarderingEmpires.Contains(notFoundEmpire))
            {
                boarderingEmpires.Remove(notFoundEmpire);

                ChangeValueInThreatRatings(notFoundEmpire, "Boardering", 0);
                notFoundEmpire.WarModule.ChangeValueInThreatRatings(thisEmpire, "Boardering", 0);

                thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(notFoundEmpire, "Boardering", 0);
                notFoundEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(thisEmpire, "Boardering", 0);
            }
        }
    }

    /*
     * The below function is used to take a tile and then to check the reasons for conquering
     * @param MapTile _tile This is the tile that you are using
     * @param MapTile _attackingTile This is the attacking tile
     * @param bool _notCheckOtherEmpire This is to prevent recursion.
     */
    public void CheckTileForReasons(MapTile _tile, MapTile _attackingTile, bool _notCheckOtherEmpire)
    {
        bool tileBoardering = false;
        foreach (var boarderingTile in _tile.GetAllConnectedTiles())
        {
            bool thisConnectionBoardering = true;
            if (boarderingTile.GetOwner() == 0)
            {
                thisConnectionBoardering = false;
            }
            else
            {
                foreach (var warEmpires in atWarEmpires)
                {
                    if (boarderingTile.GetOwner() == warEmpires.GetEmpireNumber())
                    {
                        thisConnectionBoardering = false;
                    }
                }

                foreach (var diplomacyEmpires in thisEmpire.DiplomacyModule.GetAlliedEmpires())
                {
                    if (boarderingTile.GetOwner() == diplomacyEmpires.GetEmpireNumber())
                    {
                        thisConnectionBoardering = false;
                    }
                }

                foreach (var alreadyBoardering in boarderingEmpires)
                {
                    if (boarderingTile.GetOwner() == alreadyBoardering.GetEmpireNumber())
                    {
                        thisConnectionBoardering = false;
                    }
                }
            }

            if (boarderingTile.GetOwner() == thisEmpire.GetEmpireNumber())
            {
                thisConnectionBoardering = false;
            }

            if (thisConnectionBoardering == true)
            {
                tileBoardering = true;
                break;
            }
        }

        if (_tile.GetOwner() != 0)
        {
            int surroundingTiles = 0;
            _tile.SetSurroundingTileBonus(1.0f);
            foreach (var tile in _tile.GetAllConnectedTiles())
            {
                if (_tile.GetOwner() == tile.GetOwner())
                {
                    surroundingTiles += rSurroundingTileSingle;
                    _tile.SetSurroundingTileBonus(_tile.GetSurroundingTileBonus() + surroundingTileValue);
                }
            }
            
            _tile.ChangeValueInTileReasons("SurroundingTiles", -surroundingTiles, thisEmpire);
        }

        if (tileBoardering == true)
        {
            _tile.ChangeValueInTileReasons("Boardering", -rTileBoardering, thisEmpire);
        }

        _tile.ChangeValueInTileReasons("TileReplenish", _tile.GetTroopAdding(), thisEmpire);
        _tile.ChangeValueInTileReasons("Income", _tile.GetIncome(), thisEmpire);
        _tile.ChangeValueInTileReasons("Population", _tile.GetCurrentPopulation() / rDividePopulationValue, thisEmpire);
        _tile.ChangeValueInTileReasons("CorruptPopulation", -(_tile.GetCorruptPopulation() / rDivideCorruptPopulationValue), thisEmpire);

        if (_tile.thisTileType == MapTile.TileType.Mine)
        {
            _tile.ChangeValueInTileReasons("ImportantTile", rMineTile, thisEmpire);
        }
        else if (_tile.thisTileType == MapTile.TileType.Plain)
        {
            _tile.ChangeValueInTileReasons("ImportantTile", rPlainTile, thisEmpire);
        }

        if (_tile.buildingData.GetBuildingDataOwned("Mine") == 1)
        {
            _tile.ChangeValueInTileReasons("MineBuilt", rMineBuilt, thisEmpire);
        }

        if (_tile.buildingData.GetBuildingDataOwned("Barracks") == 1)
        {
            _tile.ChangeValueInTileReasons("BarracksBuilt", rBarracksBuilt, thisEmpire);
        }

        if (_tile.buildingData.GetBuildingDataOwned("Fort") == 1)
        {
            _tile.ChangeValueInTileReasons("FortBuilt", rFortBuilt, thisEmpire);
        }

        //Checking to see if it is likely that another empire may attack you soon
        if (_tile.GetOwner() == 0)
        {
            if (thisEmpire.DiplomacyModule.EmpireInDanger() == true)
            {
                _tile.ChangeValueInTileReasons("Attacked", -rAttacked, thisEmpire);
            }
        }

        if (atWarEmpires.Count > 0)
        {
            if (_attackingTile.GetTroopPresent() > _tile.GetTroopPresent())
            {
                _tile.ChangeValueInTileReasons("YourTroops", -rYourTroops, thisEmpire);
            }
            else
            {
                _tile.ChangeValueInTileReasons("YourTroops", rYourTroops, thisEmpire);
            }
        }

        if (_attackingTile.GetTroopPresent() > _tile.GetTroopPresent())
        {
            _tile.ChangeValueInTileReasons("OverwhelmingTroops", rOverwhelmingTroops, thisEmpire);
        }

        //Do a check for if another empire is likely to conquer this tile - must consider if the tile is owned by another empire then check thier troops.
        if (_notCheckOtherEmpire == false) 
        {
            MapTile lowestEnemyTile = null;
            MapTile secondLowestTile = null;
            int tileReasonValue = 0;

            foreach (var boarderingTile in _tile.GetAllConnectedTiles())
            {
                if (boarderingTile.GetOwner() != 0 && boarderingTile.GetOwner() != thisEmpire.GetEmpireNumber())
                {
                    if (lowestEnemyTile == null)
                    {
                        lowestEnemyTile = boarderingTile;

                        EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(boarderingTile.GetOwner());
                        otherEmpire.WarModule.CheckTileForReasons(_tile, boarderingTile, true);
                        tileReasonValue = boarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire);
                    }
                    else
                    {
                        EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(boarderingTile.GetOwner());
                        otherEmpire.WarModule.CheckTileForReasons(_tile, boarderingTile, true);
                        if (tileReasonValue < boarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire))
                        {
                            secondLowestTile = lowestEnemyTile;

                            lowestEnemyTile = boarderingTile;
                            tileReasonValue = boarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire);
                        }
                    }
                }
                foreach (var secondBoarderingTile in boarderingTile.GetAllConnectedTiles())
                {
                    if (secondBoarderingTile.GetOwner() != 0 && secondBoarderingTile.GetOwner() != thisEmpire.GetEmpireNumber())
                    {
                        if (lowestEnemyTile == null)
                        {
                            lowestEnemyTile = secondBoarderingTile;

                            EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(secondBoarderingTile.GetOwner());
                            otherEmpire.WarModule.CheckTileForReasons(_tile, secondBoarderingTile, true);
                            tileReasonValue = secondBoarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire);
                        }
                        else
                        {
                            EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(secondBoarderingTile.GetOwner());
                            otherEmpire.WarModule.CheckTileForReasons(_tile, secondBoarderingTile, true);
                            if (tileReasonValue < secondBoarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire))
                            {
                                secondLowestTile = lowestEnemyTile;

                                lowestEnemyTile = secondBoarderingTile;
                                tileReasonValue = secondBoarderingTile.UpdateTileReasonsOfAllEmpires(otherEmpire);
                            }
                        }
                    }
                }
            }

            if (_tile == lowestEnemyTile || _tile == secondLowestTile)
            {
                _tile.ChangeValueInTileReasons("YourTroops", -rOtherEmpireConquer, thisEmpire);
            }
        }
    }

    /*
     * The below function will get all the garrisoned troops present within the empire
     */ 
    public int GetAllGarrisons()
    {
        int totalGarrison = 0;
        foreach (var tile in thisEmpire.GetOwnedTiles())
        {
            totalGarrison += tile.GetTroopPresent();
        }
        return totalGarrison;
    }

    /*
     * This function is called when another empire has been defeated and all refrences to it should be destroyed
     * @param EmpireClass _deadEmpire This is the empire that has died
     */
    public void OtherEmpireDied(EmpireClass _deadEmpire)
    {
        if (atWarEmpires.Contains(_deadEmpire))
        {
            atWarEmpires.Remove(_deadEmpire);
        }
        if (boarderingEmpires.Contains(_deadEmpire))
        {
            boarderingEmpires.Remove(_deadEmpire);
        }
        if (allEmpiresInGame.Contains(_deadEmpire))
        {
            allEmpiresInGame.Remove(_deadEmpire);
        }
        if (thisEmpire.DiplomacyModule.GetAlliedEmpires().Contains(_deadEmpire))
        {
            thisEmpire.DiplomacyModule.GetAlliedEmpires().Remove(_deadEmpire);
        }
    }

    /*
     * The below function occurs when an empire has declared war on you
     * @param EmpireClass _empireThatDeclaredWar The empire that has declared war on you
     */
    public void EmpireAtWarWith(EmpireClass _empireThatDeclaredWar)
    {
        if (atWarEmpires.Contains(_empireThatDeclaredWar) || thisEmpire.DiplomacyModule.GetAlliedEmpires().Contains(_empireThatDeclaredWar) || _empireThatDeclaredWar == thisEmpire)
        {
            return;
        }

        //Need to break alliances
        //If they have overlapping alliances then the alliance with the more liked empire will remain or if same with the defender

        List<EmpireClass> yourAllies = thisEmpire.DiplomacyModule.GetAlliedEmpires();
        List<EmpireClass> thierAllies = _empireThatDeclaredWar.DiplomacyModule.GetAlliedEmpires();

        List<EmpireClass> matchingAlliances = new List<EmpireClass>();
        foreach (var yourAlly in yourAllies)
        {
            foreach (var thierAlly in thierAllies)
            {
                if (thierAlly.GetEmpireNumber() == yourAlly.GetEmpireNumber()) //Same Ally
                {
                    matchingAlliances.Add(thierAlly);
                }
            }
        }

        foreach (var yourAlly in matchingAlliances)
        {
            int yourLiking = yourAlly.DiplomacyModule.GetThisEmpireOpinion(thisEmpire);
            int thierLiking = yourAlly.DiplomacyModule.GetThisEmpireOpinion(_empireThatDeclaredWar);

            if (yourLiking > thierLiking)
            {
                _empireThatDeclaredWar.DiplomacyModule.BreakAlliance(yourAlly);
                yourAlly.DiplomacyModule.BreakAlliance(_empireThatDeclaredWar);
            }
            else //If less or equal as you are the attacking empire
            {
                thisEmpire.DiplomacyModule.BreakAlliance(yourAlly);
                yourAlly.DiplomacyModule.BreakAlliance(thisEmpire);
            }
        }

        if (!atWarEmpires.Contains(_empireThatDeclaredWar))
        {
            if (thisEmpire != _empireThatDeclaredWar)
            {
               // Debug.Log("To War" + thisEmpire.GetEmpireColor() + _empireThatDeclaredWar.GetEmpireColor());
                atWarEmpires.Add(_empireThatDeclaredWar);
            }
            foreach (EmpireClass empire in _empireThatDeclaredWar.DiplomacyModule.GetAlliedEmpires())
            {
                if (!atWarEmpires.Contains(empire))
                {
                    if (thisEmpire != empire)
                    {
                       // Debug.Log("To War" + thisEmpire.GetEmpireColor() + empire.GetEmpireColor());
                        atWarEmpires.Add(empire);
                    }
                }
            }
        }
    }
    /*
     * The below function is used to check if the AI should go to war with a neighbouring faction
     */
    public EmpireClass GoToWarCheck()
    {
        foreach (var empire in boarderingEmpires)
        {
            if (empire != thisEmpire)
            {
                if (!thisEmpire.DiplomacyModule.GetAlliedEmpires().Contains(empire) && !atWarEmpires.Contains(empire)) // Do not go to war with allies unless alliance is broken first
                {
                    // Get the diplomacy and check if you have a negative relationship also check the threat rating
                    if (thisEmpire.DiplomacyModule.GetThisEmpireOpinion(empire) <= warDiplomacyNumber) // Check if you are stronger then them
                    {
                        return empire;
                    }
                }
            }
        }
        return null;
    }

    /*
     * This function is used to make peace with another empire
     * @param EmpireClass _makePeaceEmpire This is the empire you will be making peace with
     */ 
    public void MakePeace(EmpireClass _makePeaceEmpire)
    { 

        bool removeEmpirePeace = false;
        foreach (var otherEmpire in atWarEmpires)
        {
            if (otherEmpire == _makePeaceEmpire)
            {
                removeEmpirePeace = true;
            }
        }
        if (removeEmpirePeace == true)
        {
           // Debug.Log("MakePeace" + thisEmpire.GetEmpireColor() + _makePeaceEmpire.GetEmpireColor());
            atWarEmpires.Remove(_makePeaceEmpire);

            foreach (var empire in _makePeaceEmpire.DiplomacyModule.GetAlliedEmpires())
            {
                if (atWarEmpires.Contains(empire))
                {
                   // Debug.Log("MakePeace" + thisEmpire.GetEmpireColor() + empire.GetEmpireColor());
                    atWarEmpires.Remove(empire);
                    empire.WarModule.MakePeace(thisEmpire);

                }
            }
        }
        StartCoroutine(WarExhaustionEnd(_makePeaceEmpire));
    }

    private IEnumerator WarExhaustionEnd(EmpireClass _makePeaceEmpire)
    {
        yield return new WaitForSeconds(100f * delay + 1f);
        thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(_makePeaceEmpire, "WarExhaustion", 0);
    }

    /*
     * The below function will return the boardering empire value
     * @return List<EmpireClass> boarderingEmpires This is a list of all boardering empires
     */
    public List<EmpireClass> GetBoarderingEmpires()
    {
        return boarderingEmpires;
    }

    /*
     * The below function will return all the at war Empires
     * @return List<EmpireClass> atWarEmpires These are all the empires you are at war with
     */
    public List<EmpireClass> GetAtWarEmpires()
    {
        return atWarEmpires;
    }

    /*
     * The below will set the empires troop number
     * @param int _setTroopNumber This is the new troop number amount
     */
    public void SetTroopNumber(int _setTroopNumber)
    {
        if (_setTroopNumber < 0)
        {
            troopNumber = 0;
        }
        else
        {
            troopNumber = _setTroopNumber;
        }
    }

    /*
     * This will return the amount of troops you have
     * @return int troopNumber The amount of troops you have
     */ 
    public int GetTroopNumber()
    {
        return troopNumber;
    }

    /*
     * This function will return the replenish amount of the empire
     * @return int troopReplenishAmount This is the amount of troops that the empire replenishes
     */ 
    public int GetReplinishAmount()
    {
        return troopReplenishAmount;
    }

    /*
     * This returns a list of all the empires in the game
     * @return List<EmpireClass> allEmpiresInGame This is a list of all the empires in the game
     */ 
    public List<EmpireClass> GetAllEmpiresInGame()
    {
        return allEmpiresInGame;
    }

    /*
     * The below function is used to get the specific threat rating of another empire
     * @param EmpireClass _otherEmpire This is the other empire that you want to get the threat rating of
     */ 
    public int GetThreatRating(EmpireClass _otherEmpire)
    {
        return threatRatings[_otherEmpire];
    }

    /*
     * This will return the number at which an empire will consider going to war for low diplomacy.
     * @return int warDiplomacyNumber This is the value at which an empire will consider going to war
     */
    public int GetWarDiplomacyValue()
    {
        return warDiplomacyNumber;
    }

    /*
     * This will return the number at which this empire will consider another empire to be a threat
     * @return int threatValue This is the value at which this empire will consider another empire to be threat.
     */ 
    public int GetThreatValue()
    {
        return threatValue;
    }

    /*
     * The below function can be used to get the threat rating of an empire as well as its allies
     * @param EmpireClass _otherEmpire This is the empire whos threat rating along with its allies will be returned.
     */ 
    public int GetAllAllianceThreatRating(EmpireClass _otherEmpire)
    {
        int totalThreatValue = 0;
        totalThreatValue = threatRatings[_otherEmpire];
        foreach (var alliedEmpire in _otherEmpire.DiplomacyModule.GetAlliedEmpires())
        {
            if (alliedEmpire != thisEmpire)
            {
                totalThreatValue += threatRatings[alliedEmpire];
            }
        }

        return totalThreatValue;
    }

    /*
    * The below function can be used to get the all the troops + your alliance troops as well.
    */
    public int GetAllTroopsIncludingAlliances()
    {
        int totalTroops = troopNumber;
        foreach (var alliedEmpire in thisEmpire.DiplomacyModule.GetAlliedEmpires())
        {
            totalTroops += alliedEmpire.WarModule.GetTroopNumber();
        }
        return totalTroops;
    }

    /*
     * Sets a new value for the delay float
     * @param float _newDelayValue This is the new delay value.
     */ 
    public void SetDelayValue(float _newDelayValue)
    {
        delay = _newDelayValue;
    }
}
