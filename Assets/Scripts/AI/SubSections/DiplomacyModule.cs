using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for the dipplomacy of an empire.
*/

public class DiplomacyModule : MonoBehaviour
{

    private EmpireClass thisEmpire;
    private List<EmpireClass> alliedEmpire;
    private Dictionary<EmpireClass, int> thisEmpireOpinions = new Dictionary<EmpireClass, int>(); // This empires opinions of other empire
    private Dictionary<EmpireClass, int> opinionsOfYou = new Dictionary<EmpireClass, int>(); // other empire opinions of you

    //Favour
    private bool gainedFavour = false;
    private float giveFavourTimer = 30;
    private float currentFavourTimer = 0;

    private void Awake()
    {
        alliedEmpire = new List<EmpireClass>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gainedFavour == true)
        {
            currentFavourTimer += Time.deltaTime;
        }

        if (currentFavourTimer > giveFavourTimer)
        {
            gainedFavour = false;
        }
    }

    //This is so the script knows which empire it is.
    private void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;
    }

    //The below function will occur when another empire has been met for the first time.
    private void MetEmpire(EmpireClass _empire)
    {
        thisEmpireOpinions[_empire] = 0;
        opinionsOfYou[_empire] = 0;
    }


    //This will create an alliance with this empire
    private void MakeAlliance(EmpireClass _empire)
    {
        alliedEmpire.Add(_empire);
        //They should join any wars that you have
    }

    //The below function is used to increase your favour with another Empire
    private void GainFavour(EmpireClass _empire)
    {
        _empire.DiplomacyModule.IncreaseOpinion(thisEmpire, 20);
        IncreaseOtherOpinion(_empire, 20);
    }

    //The below function is used to gift money to another empire so that thier opinion of you is increased
    private void GiftMoney(EmpireClass _empire)
    {

    }

    //The below function will be used to give tiles to another Empire
    private void GiveTile(EmpireClass _empire, MapTile _tileToGive)
    {
        _tileToGive.SetOwner(_empire.GetEmpireNumber());
    }

    //The below function will increase another empires opinion of you by a set amount
    private void IncreaseOpinion(EmpireClass _empire, int _increaseBy)
    {
        if (_increaseBy > 0)
        {
            if (opinionsOfYou[_empire] < 100)
            {
                opinionsOfYou[_empire] = opinionsOfYou[_empire] + _increaseBy;
            }
        }
        else if (_increaseBy < 0)
        {
            if (opinionsOfYou[_empire] > -100)
            {
                opinionsOfYou[_empire] = opinionsOfYou[_empire] + _increaseBy;
            }
        }
    }

    //The below opinion will set the opinion of another empire to a specific number
    private void SetOpinion(EmpireClass _empire, int _setTo)
    {
         opinionsOfYou[_empire] = _setTo;
    }

    //The below function will retun the empires opinion of the you
    private int ReturnEmpireOpinion(EmpireClass _empire)
    {
        return opinionsOfYou[thisEmpire];
    }

    // The below function will incrase your opinion of an empire by an amount
    private void IncreaseOtherOpinion(EmpireClass _empire, int _increaseBy)
    {
        if (_increaseBy > 0)
        {
            if (thisEmpireOpinions[_empire] < 100)
            {
                thisEmpireOpinions[_empire] = thisEmpireOpinions[_empire] + _increaseBy;
            }
        }
        else if (_increaseBy < 0)
        {
            if (thisEmpireOpinions[_empire] > -100)
            {
                thisEmpireOpinions[_empire] = thisEmpireOpinions[_empire] + _increaseBy;
            }
        }
    }

    //The below function will specifically set your opinion of an empire to a number
    private void SetOtherOpinion(EmpireClass _empire, int _setTo)
    {
        thisEmpireOpinions[_empire] = _setTo;
    }


    //The below function will return your opinion of a empire
    private int ReturnOtherEmpireOpinion(EmpireClass _empire)
    {
        return thisEmpireOpinions[_empire];
    }

}
