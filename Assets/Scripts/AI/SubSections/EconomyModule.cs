using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for the economy of an empire.
*/

public class EconomyModule : MonoBehaviour
{

    private EmpireClass thisEmpire;

    private int totalAmountOfMoney;
    private int moneyUpdateAmount;

    private bool trainTroops = true;

    private Dictionary<string, Dictionary<string, int>> buildingReasons = new Dictionary<string, Dictionary<string, int>>(); // These are the reasons that you may want to make a particular type of building.

    private List<string> buildingNames = new List<string>();

    private int surplasValue = 0;

    private float negativeTime = 0;

    //Building Reasons
    private int rTraining = 20;
    private int rSpecialTiles = 20;
    private int rInDanger = 20;
    private int rSurplas = 20;
    private int rAtWar = 20;
    private int rCanBuild = -100;

    /*
    * This is so the script knows which empire it is.
    * @param EmpireClass _thisEmpire The empire this script is attached to
    */
    public void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;

        SetUpAllBuildingReasons();
    }


    /*
    * The below function is used to set up the reasons to build a specific building
    */
    public void SetUpAllBuildingReasons()
    {
        buildingReasons["Fort"] = new Dictionary<string, int>();
        buildingReasons["Fort"]["SpecialTiles"] = 0; // This is if you have special tiles that you want protected
        buildingReasons["Fort"]["InDanger"] = 0; // This is if you are likely to be attacked by an empire 
        buildingReasons["Fort"]["CanBuild"] = 0; // This is if there is a tile for you to build this on

        buildingReasons["Mine"] = new Dictionary<string, int>();
        buildingReasons["Mine"]["Training"] = 0; // This is if you are currently training troops
        buildingReasons["Mine"]["Surplus"] = 0; //This is if you have a large amount of money
        buildingReasons["Fort"]["CanBuild"] = 0; // This is if there is a tile for you to build this on

        buildingReasons["Barracks"] = new Dictionary<string, int>();
        buildingReasons["Barracks"]["AtWar"] = 0; //This is if you are at war with an Empire
        buildingReasons["Barracks"]["Training"] = 0; //This if you are currently training troops
        buildingReasons["Fort"]["CanBuild"] = 0; // This is if there is a tile for you to build this on

        buildingNames.Add("Barracks");
        buildingNames.Add("Mine");
        buildingNames.Add("Fort");
    }


    /*
    * The below will loop through all the reasons and update the building reasons for a specific building
    * @param string _building This is what type of building you want to get the reasons for
    * @retun int total This is all the added up reasons to conquer this tile
    */
    public int UpdateAllBuildingReasons(string _building)
    {
        int total = 0;
        if (_building == "Fort")
        {
            total += buildingReasons[_building]["SpecialTiles"];
            total += buildingReasons[_building]["InDanger"];
        }
        else if (_building == "Mine")
        {
            total += buildingReasons[_building]["Training"];
            total += buildingReasons[_building]["Surplus"];
        }
        else if (_building == "Barracks")
        {
            total += buildingReasons[_building]["AtWar"];
            total += buildingReasons[_building]["Training"];
        }
        else
        {
            Debug.LogWarning("This should not be happening ");
        }

        return total;
    }


    /*
    * The below function is used to update the value for a reason why the reasons to build a building has increased or decreased.
    * @param string _reason This is the reason that the tile reason is increasing or decreasing
    * @param int _newValue This is the new value that it will be set to.
    * @param string _building This is what type of building you want to get the reasons for
    */
    public void ChangeValueInBuildingReasons(string _reason, int _newValue, string _building)
    {
        buildingReasons[_building][_reason] = _newValue;
    }

    /*
     * The below function is used to check if this empire should build a building and if so then the empire will build it.
     */
    public void BuildBuilding()
    {
        if (trainTroops == true)
        {
            ChangeValueInBuildingReasons("Training", rTraining, "Mine");
            ChangeValueInBuildingReasons("Training", -rTraining, "Barracks");
        }
        else
        {
            ChangeValueInBuildingReasons("Training", -rTraining, "Mine");
            ChangeValueInBuildingReasons("Training", rTraining, "Barracks");
        }

        if (thisEmpire.GetAllTilesWithSpecialBuildings() != null)
        {
            ChangeValueInBuildingReasons("SpecialTiles", rSpecialTiles, "Fort");
        }
        else
        {
            ChangeValueInBuildingReasons("SpecialTiles", -rSpecialTiles, "Fort");
        }

        if (thisEmpire.DiplomacyModule.EmpireInDanger())
        {
            ChangeValueInBuildingReasons("InDanger", rInDanger, "Fort");
        }
        else
        {
            ChangeValueInBuildingReasons("InDanger", -rInDanger, "Fort");
        }

        if (totalAmountOfMoney > surplasValue)
        {
            ChangeValueInBuildingReasons("InDanger", rSurplas, "Mine");
        }
        else
        {
            ChangeValueInBuildingReasons("InDanger", -rSurplas, "Mine");
        }

        if (thisEmpire.WarModule.GetAtWarEmpires().Count > 0)
        {
            ChangeValueInBuildingReasons("AtWar", rAtWar, "Barracks");
        }
        else
        {
            ChangeValueInBuildingReasons("AtWar", -rAtWar, "Barracks");
        }

         if (thisEmpire.GetSafestTypeTile(MapTile.TileType.Mine, "Mine") == null)
        {
            ChangeValueInBuildingReasons("CanBuild", rCanBuild, "Mine");
        }

        if (thisEmpire.GetSafestTypeTile(MapTile.TileType.Plain, "Barracks") == null)
        {
            ChangeValueInBuildingReasons("CanBuild", rCanBuild, "Barracks");
        }

        if (thisEmpire.GetBoarderTilesWithThreateningEmpire() == null)
        {
            ChangeValueInBuildingReasons("CanBuild", rCanBuild, "Fort");
        }

         int highestNumber = 0;
         string highestName = null;
         foreach (var name in buildingNames)
         {
             if (highestName == null)
             {
                 highestName = name;
                 highestNumber = UpdateAllBuildingReasons(name);
             }
             else
             {
                 if (highestNumber < UpdateAllBuildingReasons(name))
                 {
                     highestName = name;
                     highestNumber = UpdateAllBuildingReasons(name);
                 }
             }
         }
         if (highestNumber > 0)
         {
            MapTile tile = null;
            if (highestName == "Mine")
            {
                tile = thisEmpire.GetSafestTypeTile(MapTile.TileType.Mine, "Mine");
            }
            else if (highestName == "Barracks")
            {
                tile = thisEmpire.GetSafestTypeTile(MapTile.TileType.Plain, "Barracks");
            }
            else if (highestName == "Fort")
            {
                tile = thisEmpire.GetBoarderTilesWithThreateningEmpire();
            }

            if (tile != null)
            {
                if (totalAmountOfMoney > thisEmpire.WarModule.GetTroopNumber() + tile.buildingData.GetBuildingDataPrice(highestName) * 1.5)
                {
                    Debug.Log(thisEmpire.GetEmpireNumber().ToString() + highestName + tile.GetTileNumber().ToString());
                    if (tile.buildingData.GetBuildingDataOwned(highestName) == 0)
                    {
                        tile.buildingData.ChangeDataOwned(highestName, 1);
                        SetCurrentMoney(totalAmountOfMoney - tile.buildingData.GetBuildingDataPrice(highestName));
                        tile.BuildingBuilt(highestName);
                    }
                }
            }
         }
    }

    /*
     * The below function is used in AIMain to update the total amount of money that this empire has
     */
    public void UpdateEmpireMoney()
    {
        totalAmountOfMoney += moneyUpdateAmount;
        int troopAmount = thisEmpire.WarModule.GetTroopNumber();
        totalAmountOfMoney = totalAmountOfMoney - troopAmount;

        if (totalAmountOfMoney <= 0)
        {
            negativeTime += Time.deltaTime;
        }
        else
        {
            if (negativeTime > 0)
            {
                negativeTime = 0;
            }
        }
        surplasValue = (troopAmount * 10) + 100;

        if (negativeTime > 0)
        {
            int totalCorruptPopulation = 0;
            foreach (var tile in thisEmpire.GetOwnedTiles())
            {
                if (tile.GetCorruptPopulation() > 0)
                {
                   // tile.SetCorruptPopulation((tile.GetCorruptPopulation() * 1.02f) + negativeTime * 20);
                    totalCorruptPopulation += tile.GetCorruptPopulation();
                }
            }
        }
    }

    /*
     * The below function is used to calulate what the empires total money amount should be from scratch.
     */ 
    public void CalculateMoneyUpdateAmount()
    {
        moneyUpdateAmount = 0;
        List<MapTile> yourTiles = thisEmpire.GetOwnedTiles();
        foreach (MapTile tile in yourTiles)
        {
            moneyUpdateAmount += tile.GetIncome() + tile.GetCurrentPopulation()/20 - tile.GetCorruptPopulation()/15;
        }
    }

    /*
     * The below function is used to set the current amount of money that the empire currently has.
     * @param int _newTotalMoney This is the new value for how much money the empire has.
     */ 
    public void SetCurrentMoney(int _newTotalMoney)
    {
        _newTotalMoney = totalAmountOfMoney;
    }

    /*
     * The below function will return the totalAmountOfMoney that the empire has
     * @return int totalAmountOfMoney this is the amount of money that the empire currently has
     */ 
    public int GetCurrentMoney()
    {
        return totalAmountOfMoney;
    }

    /*
     * The below function is used to set the current moneyUpdateAmount.
     * @param int _newMoneyValue This is the new value that you want to set for moneyUpdateAmount
     */ 
    public void SetIncomeValue(int _newMoneyValue)
    {
        _newMoneyValue = moneyUpdateAmount;
    }

    /*
     * The below function is used to get the moneyUpdateAmount value
     * @return int moneyUpdateAmount This is how much money the empire will recieve every turn
     */ 
    public int GetIncomeValue()
    {
        return moneyUpdateAmount;
    }

    /*
     * This fuction is to set the train troops to a new value
     * @param bool _newTrainTroops This is the new value of the train troops
     */ 
    public void SetTrainTroops(bool _newTrainTroops)
    {
        trainTroops = _newTrainTroops;
    }

    /*
     * The below function is used to return the train troops bool value
     */ 
    public bool GetTrainTroops()
    {
        return trainTroops;
    }

    /*
     * The below function will return the current negative time of an empire which is how long they have been in debt
     */ 
    public float GetNegativeTime()
    {
        return negativeTime;
    }
}
