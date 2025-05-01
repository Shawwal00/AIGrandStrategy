using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This script is responsible for anything that takes place inside of an empire such as population, or corruption.
 */

public class InternalModule : MonoBehaviour
{
    private EmpireClass thisEmpire;

    private Dictionary<string, int> trainTroopsReasons = new Dictionary<string, int>(); // These are the reasons that this empire might train or stop training troops

    private int changeTrainTroops = 0;

    private float updatePopulationTime = 0;

    private bool notEnoughAmeneties = false;

    private int corruptionDivder = 35;

    //Train Troops reasons
    private int rNegative = 50;
    private int rDuration = 20;
    private int rAtWar = 20;
    private int rPositiveIncome = 40;
    private int rSmallEmpire = 80;


    /*
    * This is so the script knows which empire it is.
    * @param EmpireClass _thisEmpire The empire this script is attached to
    */
    public void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;

        SetUpAllTrainTroopReasons();
    }

    /*
     * The below function is used to update the population of an empire
     */
    public void UpdatePopulation()
    {
        int totalPopulation = 0;
        int totalAmeneties = 0;
        foreach (var tile in thisEmpire.GetOwnedTiles())
        {
            tile.SetCurrentPopulation(tile.GetCurrentPopulation() + tile.GetAddingPopulation());
            totalPopulation += tile.GetCurrentPopulation();
            totalAmeneties += tile.GetAmeneties();
        }
        if (totalAmeneties * 100 < totalPopulation)
        {
            if (notEnoughAmeneties == false)
            {
                notEnoughAmeneties = true;
                foreach (var empire in thisEmpire.DiplomacyModule.GetMetEmpires())
                {
                    thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(empire, "Ameneties", thisEmpire.DiplomacyModule.GetAmenetiesReason());
                }
                thisEmpire.SetPopulationMigrating(true);
            }
        }
        else
        {
            if (notEnoughAmeneties == true)
            {
                notEnoughAmeneties = false;
                foreach (var empire in thisEmpire.DiplomacyModule.GetMetEmpires())
                {
                    thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(empire, "Ameneties", 0);
                }
                thisEmpire.SetPopulationMigrating(false);
            }
            
        }
    }

    /*
     * The below function is used to go through and update anything relating to this module.
     */
    public void UpdateInternals()
    {
        if (thisEmpire.EconomyModule.GetCurrentMoney() < 0)
        {
            ChangeValueInTrainTroopReasons("Negative", -rNegative);
        }
        else 
        {
            ChangeValueInTrainTroopReasons("Negative", rNegative);
        }

        if (thisEmpire.EconomyModule.GetNegativeTime() > 0)
        {
            ChangeValueInTrainTroopReasons("Duration", -rDuration - ((int)thisEmpire.EconomyModule.GetNegativeTime() / 5));
        }
        else
        {
            ChangeValueInTrainTroopReasons("Duration", rDuration);
        }

        if (thisEmpire.WarModule.GetAtWarEmpires().Count > 0)
        {
            ChangeValueInTrainTroopReasons("AtWar", rAtWar);
        }
        else 
        {
            ChangeValueInTrainTroopReasons("AtWar", -rAtWar);
        }

        if(thisEmpire.EconomyModule.GetIncomeValue() > thisEmpire.WarModule.GetTroopNumber())
        {
            ChangeValueInTrainTroopReasons("PositiveIncome", rPositiveIncome);
        }
        else
        {
            ChangeValueInTrainTroopReasons("PositiveIncome", -rPositiveIncome);
        }

        if (thisEmpire.GetOwnedTiles().Count <= 5)
        {
            ChangeValueInTrainTroopReasons("SmallEmpire", rSmallEmpire);
        }
        else
        {
            ChangeValueInTrainTroopReasons("SmallEmpire", 0);
        }

        if (UpdateTrainTroopReasons() >= changeTrainTroops && thisEmpire.EconomyModule.GetTrainTroops() == false)
        {
            thisEmpire.EconomyModule.SetTrainTroops(true);
        }
        else if (UpdateTrainTroopReasons() < changeTrainTroops && thisEmpire.EconomyModule.GetTrainTroops() == true)
        {
            thisEmpire.EconomyModule.SetTrainTroops(false);
        }

       /* if (updatePopulationTime == 0)
        {
            UpdatePopulation();
        }
        else
        {
            updatePopulationTime += Time.deltaTime;
            if (updatePopulationTime > 2)
            {
                updatePopulationTime = 0;
            }
        }*/

        UpdatePopulation();

        int totalCorruptPopulation = 0;
        foreach (var tile in thisEmpire.GetOwnedTiles())
        {
            if (tile.GetCorruptPopulation() > 0)
            {
                tile.SetCorruptPopulation(tile.GetCorruptPopulation() * 0.99f);
                totalCorruptPopulation += tile.GetCorruptPopulation();
            }
        }

        if (totalCorruptPopulation > 0)
        {
            foreach (var tile in thisEmpire.GetOwnedTiles())
            {
                if ((int)thisEmpire.EconomyModule.GetNegativeTime() > 0)
                {
                    tile.SetTroopPresent(tile.GetTroopPresent() - tile.GetCorruptPopulation() / corruptionDivder - (int)thisEmpire.EconomyModule.GetNegativeTime() * 10);
                    thisEmpire.WarModule.SetTroopNumber(thisEmpire.WarModule.GetTroopNumber() - tile.GetTroopPresent() - tile.GetCorruptPopulation() / corruptionDivder - (int)thisEmpire.EconomyModule.GetNegativeTime() * 10);

                }
                else
                {
                    tile.SetTroopPresent(tile.GetTroopPresent() - tile.GetCorruptPopulation() / corruptionDivder);
                    thisEmpire.WarModule.SetTroopNumber(thisEmpire.WarModule.GetTroopNumber() - tile.GetTroopPresent() - tile.GetCorruptPopulation() / corruptionDivder);
                }
            }
        }

        thisEmpire.FunctionFinished();
    }

    /*
    * The below function is used to set up the reasosns why this empire might train troops.
    */
    public void SetUpAllTrainTroopReasons()
    {
        trainTroopsReasons["Negative"] = 0; // This is if the empire has a negative amount of money
        trainTroopsReasons["Duration"] = 0; // This is if the empire is negative and how long it has been for
        trainTroopsReasons["AtWar"] = 0; // This is if the empire is at war currently
        trainTroopsReasons["PositiveIncome"] = 0; // This is if you income is less then your expenditure
        trainTroopsReasons["SmallEmpire"] = 0; // This is if you are a small empire less then 5 tiles
    }

    /*
    * The below will loop through all the reasons and update the train troop reasons
    * @retun int total This is all the added up reasons to conquer this tile
    */
    public int UpdateTrainTroopReasons()
    {
        int total = 0;
        total += trainTroopsReasons["Negative"];
        total += trainTroopsReasons["Duration"];
        total += trainTroopsReasons["AtWar"];
        total += trainTroopsReasons["PositiveIncome"];
        total += trainTroopsReasons["SmallEmpire"];

        return total;
    }

    /*
    * The below function is used to update the value for a train troop reason.
    * @param string _reason This is the reason that the tile reason is increasing or decreasing
    * @param int _newValue This is the new value that it will be set to.
    */
    public void ChangeValueInTrainTroopReasons(string _reason, int _newValue)
    {
        trainTroopsReasons[_reason] = _newValue;
    }

    /*
     * The below gets the corruptionDivider int
     * @return int corruptionDivider this is the rate at which troops will leave.
     */ 
    public int GetCorruptionDivider()
    {
        return corruptionDivder;
    }
}
