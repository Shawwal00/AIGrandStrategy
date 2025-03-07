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

    /*
    * This is so the script knows which empire it is.
    * @param EmpireClass _thisEmpire The empire this script is attached to
    */
    public void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;
    }

    public void BuildBuilding()
    {
        
    }

    /*
     * This function is used by the AI to check if they should continue to train troops or if they should stop.
     */ 
    public void CheckTrainTroops()
    {
        //Check if at war - do max
        //If not at war then do half

        int troopAmount = thisEmpire.WarModule.GetTroopNumber();

        if (thisEmpire.WarModule.GetAtWarEmpires().Count > 0)
        {
            if (moneyUpdateAmount > troopAmount)
            {
                trainTroops = true;
            }
            else
            {
                trainTroops = false;
            }
        }
        else 
        {
            if (thisEmpire.ReturnOwnedTiles().Count > 5)
            {
                if (moneyUpdateAmount / 2 > troopAmount)
                {
                    trainTroops = true;
                }
                else
                {
                    trainTroops = false;
                }
            }
        }

        //If at war and losing then do over assuming you will not become bankrupt
    }

    /*
     * The below function is used in AIMain to update the total amount of money that this empire has
     */
    public void UpdateEmpireMoney()
    {
        CheckTrainTroops();
        totalAmountOfMoney += moneyUpdateAmount;
        int troopAmount = thisEmpire.WarModule.GetTroopNumber();
        totalAmountOfMoney = totalAmountOfMoney - troopAmount;

        if (totalAmountOfMoney <= 0)
        {
            Debug.Log("This empire should get downsides");
        }
    }

    /*
     * The below function is used to calulate what the empires total money amount should be from scratch.
     */ 
    public void CalculateMoneyUpdateAmount()
    {
        moneyUpdateAmount = 0;
        List<MapTile> yourTiles = thisEmpire.ReturnOwnedTiles();
        foreach (MapTile tile in yourTiles)
        {
            moneyUpdateAmount += tile.GetIncome();
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
     * The below function is used to return the train troops bool value
     */ 
    public bool GetTrainTroops()
    {
        return trainTroops;
    }
}
