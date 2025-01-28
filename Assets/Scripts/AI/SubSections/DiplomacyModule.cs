using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for the dipplomacy of an empire.
*/

public class DiplomacyModule : MonoBehaviour
{

    private EmpireClass thisEmpire;
    private List<EmpireClass> alliedEmpire; // A list of allied empires
    private Dictionary<EmpireClass, int> thisEmpireOpinions = new Dictionary<EmpireClass, int>(); // This empires opinions of other empire
    private Dictionary<EmpireClass, int> opinionsOfYou = new Dictionary<EmpireClass, int>(); // other empire opinions of you

    //Favour
    private bool gainedFavour = false;
    private float giveFavourTimer = 30;
    private float currentFavourTimer = 0;

    private void Awake()
    {
        //Lists
        alliedEmpire = new List<EmpireClass>();
    }

    // Update is called once per frame
    void Update()
    {
        //Rests the favour after a set amount of time
        if (gainedFavour == true)
        {
            currentFavourTimer += Time.deltaTime;
        }

        if (currentFavourTimer > giveFavourTimer)
        {
            gainedFavour = false;
        }
    }

    /*
     * This is so the script knows which empire it is.
     * @param EmpireClass _thisEmpire The empire this script is attached to
     */
    public void SetThisEmpire(EmpireClass _thisEmpire)
    {
        thisEmpire = _thisEmpire;
    }

    /*
     * The below function will occur when another empire has been met for the first time.
     * @param EmpireClass _empire This is the new empire you have just met
     */
    private void MetEmpire(EmpireClass _empire)
    {
        thisEmpireOpinions[_empire] = 0;
        opinionsOfYou[_empire] = 0;
    }


    /*
     * This will create an alliance with this empire
     * @param EmpireClass _empire The empire you are making an alliance with
     */
    private void MakeAlliance(EmpireClass _empire)
    {
        alliedEmpire.Add(_empire);
        //They should join any wars that you have
    }

    /*
     * The below function is used to increase your favour with another Empire
     * @param EmpireClass _empire This is the empire you are increasing favour with
     */
    private void GainFavour(EmpireClass _empire)
    {
        _empire.DiplomacyModule.IncreaseThisEmpireOpinion(thisEmpire, 20);
        IncreaseThisEmpireOpinion(_empire, 20);
    }

    /*
     * The below function is used to gift money to another empire so that thier opinion of you is increased
     * @param EmpireClass _empire This is the empire you are gifting money too
     */
    private void GiftMoney(EmpireClass _empire)
    {

    }

    /*
     * The below function will be used to give tiles to another Empire
     * @param EmpireClass _empire This is the empire that you are giving the tile to
     * @param MapTile _tileToGive This is the tile that is being given
     */
    private void GiveTile(EmpireClass _empire, MapTile _tileToGive)
    {
        _tileToGive.SetOwner(_empire.GetEmpireNumber());
    }

    /*
     * The below function will increase another empires opinion of you by a set amount
     * @param EmpireClass _empire This is the empire whos opinion is increasing
     * @param int _increaseBy This is how much it is increasing
     */
    private void IncreaseOpinionsOfYou(EmpireClass _empire, int _increaseBy)
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

    /*
     * The below opinion will set the opinion of another empire to a specific number
     * @param EmpireClass _empire This is the empire whos opinion of you will be inceasing
     * @param int _setTo This is what the value will be set to
     */
    private void SetOpinionsOfYou(EmpireClass _empire, int _setTo)
    {
         opinionsOfYou[_empire] = _setTo;
    }

    /*
     * The below function will retun the empires opinion of the you
     * @param EmpireClass _empire This is the empire whos opinion will be returned
     * @return int opinionsOfYou[_empire] the number that the opinion is
     */
    private int ReturnEmpireOpinionsOfYou(EmpireClass _empire)
    {
        return opinionsOfYou[_empire];
    }

    /*
     *The below function will incrase your opinion of an empire by an amount
     * @param EmpireClass _empire This is the empire whos opinion of you is increasing
     * @param int _increaseBy This is how much it is increasing
     */
    private void IncreaseThisEmpireOpinion(EmpireClass _empire, int _increaseBy)
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

    /*
     * The below function will specifically set your opinion of an empire to a number
     * @param EmpireClass _empire This is the empire whos opinion you will be inceasing
     * @param int _setTo This is what the value will be set to
     */
    private void SetThisEmpireOpinion(EmpireClass _empire, int _setTo)
    {
        thisEmpireOpinions[_empire] = _setTo;
    }


    /*
     * The below function will return your opinion of a empire
     * @param EmpireClass _empire This is the empire whos opinion will be returned
     * @return int opinionsOfYou[_empire] the number that the opinion is
     */
    private int ReturnThisEmpireOpinion(EmpireClass _empire)
    {
        return thisEmpireOpinions[_empire];
    }

}
