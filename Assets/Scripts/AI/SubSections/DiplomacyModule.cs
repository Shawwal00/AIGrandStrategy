using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * This script is responsible for the dipplomacy of an empire.
*/

public class DiplomacyModule : MonoBehaviour
{

    private EmpireClass thisEmpire;
    private List<EmpireClass> alliedEmpires; // A list of allied empires
    private Dictionary<EmpireClass, int> thisEmpireOpinions = new Dictionary<EmpireClass, int>(); // This empires opinions of other empire
                                                                                                  // private Dictionary<EmpireClass, int> opinionsOfYou = new Dictionary<EmpireClass, int>(); // other empire opinions of you
    private Dictionary<EmpireClass, Dictionary<string, int>> allReasons = new Dictionary<EmpireClass, Dictionary<string, int>>(); // These are the reasons that this empire likes or dislikes another empire.

    private int makeAllianceNumber = 40;
    private int breakAllianceNumber = -50;

    private int threatDanger = 10;
    private int opinionDanger = 10;

    private int moneyTimes = 1;

    //Favour
    private bool gainedFavour = false;
    private float giveFavourTimer = 30;
    private float currentFavourTimer = 0;

    private int strongEnough = 40; // This is the value at which this empire will consider another empire to be strong
    private int makePeace = 40; // This is the value at which an empire will make peace if they feel that another empire is stronger then them.

    //All Value Reasons
    private int rBoardering = 40;
    private int rWarDecrease = 1;
    private int rStrength = 20;
    private int rComplement = 10;
    private int rMoney = 0;
    private int rAllianceUpdate = 10;
    private int rBrokeAlliance = 50;

    private void Awake()
    {
        //Lists
        alliedEmpires = new List<EmpireClass>();

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
     * The below will loop through all the reasons and update the diplomacy of all the empires
     */
    public void UpdateDiplomacyOfAllEmpires()
    {
        for (int i = 0; i < thisEmpire.WarModule.GetBoarderingEmpires().Count; i++)
        {
            int total = 0;
            EmpireClass otherEmpire = thisEmpire.WarModule.GetBoarderingEmpires()[i];

            total += allReasons[otherEmpire]["Boardering"];
            total += allReasons[otherEmpire]["War"];
            total += allReasons[otherEmpire]["Strength"];
            total += allReasons[otherEmpire]["Complement"];
            total += allReasons[otherEmpire]["Money"];
            total += allReasons[otherEmpire]["Gift"];
            total += allReasons[otherEmpire]["Alliances"];
            total += allReasons[otherEmpire]["BrokeAlliance"];

            thisEmpireOpinions[otherEmpire] = total;
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
     * @param EmpireClass _otherEmpire This is the new empire you have just met
     */
    public void MetEmpire(EmpireClass _otherEmpire)
    {
        thisEmpireOpinions[_otherEmpire] = 0;
        allReasons[_otherEmpire] = new Dictionary<string, int>();
        allReasons[_otherEmpire]["Boardering"] = 0; // This is if the empire is boardering
        allReasons[_otherEmpire]["War"] = 0; // This is if the empire is at war
        allReasons[_otherEmpire]["Strength"] = 0; // This if the other empire is stronger or as strong as you or minus if weaker
        allReasons[_otherEmpire]["Complement"] = 0; // This is if the other empire has complemented you
        allReasons[_otherEmpire]["Money"] = 0; //This is if the other empire has the same or more money then you
        allReasons[_otherEmpire]["Gift"] = 0; //This is if the other empire has given you a gift
        allReasons[_otherEmpire]["Alliances"] = 0; // This is if the other empire has alliances that you do not like.
        allReasons[_otherEmpire]["BrokeAlliance"] = 0; // This is if the other empire has broken an alliance with you
        allReasons[_otherEmpire]["Winning"] = 0; //This is if you are currently winning the war you are in - if you are at war with them.
        // opinionsOfYou[_empire] = 0;
    }

    /*
     * The below function is used to update the value for a reason why the diplomacy total has increased or decreased.
     * @param EmpireClass _otherEmpire This is the other empire
     * @param string _reason This is the reason that the diplomacy is increasing or decreasing
     * @param int _newValue This is the new value that it will be set to.
     */
    public void ChangeValueInDiplomacyReasons(EmpireClass _otherEmpire, string _reason, int _newValue)
    {
        allReasons[_otherEmpire][_reason] = _newValue;
        UpdateDiplomacyOfAllEmpires();
    }


    /*
     * The below function is used to return the value of a reason.
     * @param EmpireClass _otherEmpire This is the other empire
     * @param string _reason This is the reason that the diplomacy is increasing or decreasing
     */
    public int GetDiplomacyReasonValue(EmpireClass _otherEmpire, string _reason)
    {
        return allReasons[_otherEmpire][_reason];
    }

    /*
     * This will create an alliance with this empire
     * @param EmpireClass _empire The empire you are making an alliance with
     */
    private void MakeAlliance(EmpireClass _empire)
    {
        if (!alliedEmpires.Contains(_empire))
        {
            Debug.Log("Made Alliance" + thisEmpire.GetEmpireColor() + _empire.GetEmpireColor());
            alliedEmpires.Add(_empire);
            _empire.DiplomacyModule.MakeAlliance(thisEmpire);
            ChangeValueInDiplomacyReasons(_empire, "BrokeAlliance", 0);
            if (thisEmpire.WarModule.GetAtWarEmpires().Count > 0)
            {
                foreach (EmpireClass atWarEmpire in thisEmpire.WarModule.GetAtWarEmpires())
                {
                    _empire.WarModule.EmpireAtWarWith(atWarEmpire);
                }
            }
        }
    }

    /*
    * This will break your alliance with this empire
    * @param EmpireClass _empire The empire you are making an alliance with
    */
    public void BreakAliiance(EmpireClass _empire)
    {
        if (alliedEmpires.Contains(_empire))
        {
            ChangeValueInDiplomacyReasons(_empire, "BrokeAlliance", -rBrokeAlliance);
            alliedEmpires.Remove(_empire);
            _empire.DiplomacyModule.BreakAliiance(thisEmpire);
            Debug.Log("Break Alliance" + thisEmpire.GetEmpireColor() + _empire.GetEmpireColor());
        }
    }

    /*
     * The below function is used to increase your favour with another Empire
     * @param EmpireClass _empire This is the empire you are increasing favour with
     */
    private void GainFavour(EmpireClass _empire)
    {
        if (gainedFavour == false)
        {
            ChangeValueInDiplomacyReasons(_empire, "Complement", rComplement);
            _empire.DiplomacyModule.ChangeValueInDiplomacyReasons(thisEmpire, "Complement", rComplement);
            gainedFavour = true;
        }
    }

    /*
     * The below function is used to gift money to another empire so that thier opinion of you is increased
     * @param EmpireClass _empire This is the empire you are gifting money too
     * @param int _ampount This is the amount of money that you want to gift to the empire - more money means they will like you more.
     */
    private void GiftMoney(EmpireClass _empire, int _amount)
    {
        thisEmpire.EconomyModule.SetCurrentMoney(thisEmpire.EconomyModule.GetCurrentMoney() - (_amount / moneyTimes));
        ChangeValueInDiplomacyReasons(_empire, "Gift", _amount/ moneyTimes);
        StartCoroutine(EndGiftBonus(_empire, _amount/ moneyTimes));

    }

    /*
    * The below function is used to end the positive effect from giving an Empire Money
    * @param EmpireClass _empire This is the empire you are gifting money too
    * @param int _ampount This is the amount of money that you want to gift to the empire - more money means they will like you more.
    */
    private IEnumerator EndGiftBonus(EmpireClass _empire, int _amount)
    {
        yield return new WaitForSeconds(0.5f * _amount);
        ChangeValueInDiplomacyReasons(_empire, "Gift", 0);
        moneyTimes += 1;
    }

    /*
     * This function will check the alliances of other empires and see if there are any that this empire likes
     */ 
    public void CheckOtherAllEmpiresAlliances()
    {
        //Get Empires you like
        List<EmpireClass> likedEmpires = new List<EmpireClass>();
        List<EmpireClass> unlikedEmpires = new List<EmpireClass>();

        foreach (var allEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
        {
            if (thisEmpireOpinions[allEmpire] > 0)
            {
                likedEmpires.Add(allEmpire);
            }
            else
            {
                unlikedEmpires.Add(allEmpire);
            }
        }

        //Check all the empires and see if you like simillar empires
        foreach (var allEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
        {
            ChangeValueInDiplomacyReasons(allEmpire, "Alliances", 0);
            foreach (var secondAllEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
            {
                int opinion = allEmpire.DiplomacyModule.GetThisEmpireOpinion(secondAllEmpire);
                if (opinion > 0)
                {
                    foreach (EmpireClass likedEmpire in likedEmpires)
                    {
                        if (secondAllEmpire.GetEmpireNumber() == likedEmpire.GetEmpireNumber())
                        {
                            ChangeValueInDiplomacyReasons(allEmpire, "Alliances", GetDiplomacyReasonValue(allEmpire, "Alliances") + rAllianceUpdate);
                        }
                    }

                    foreach (EmpireClass unlikedEmpire in unlikedEmpires)
                    {
                        if (secondAllEmpire.GetEmpireNumber() == unlikedEmpire.GetEmpireNumber())
                        {
                            ChangeValueInDiplomacyReasons(allEmpire, "Alliances", GetDiplomacyReasonValue(allEmpire, "Alliances") - rAllianceUpdate);
                        }
                    }
                }
            }
        }
    }

    /*
     * The below function is used to update important diplomacy actions 
     */
    public void UpdateDiplomacy()
    {
        CheckOtherAllEmpiresAlliances();
        EmpireClass empireToImproveRelations = null;
        int amountToIncrease = 0;

        foreach (var allEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
        {
            // Check if the other empires are strong
            if (thisEmpire.WarModule.GetThreatRating(allEmpire) > strongEnough)
            {
                ChangeValueInDiplomacyReasons(allEmpire, "Stength", rStrength);
            }
            else
            {
                ChangeValueInDiplomacyReasons(allEmpire, "Stength", -rStrength);
            }
        }

        //See if you can make peace with the atWarEmpires
        List<EmpireClass> empiresToMakePeaceWith = new List<EmpireClass>();
        foreach (var atWarEmpire in thisEmpire.WarModule.GetAtWarEmpires())
        {
            //Check to see if you like the empire
            // If not then see if you are weaker - Try to make the other empire like you.

            if ((thisEmpireOpinions[atWarEmpire] > makePeace || EmpireInDanger() == true ) && (atWarEmpire.DiplomacyModule.thisEmpireOpinions[thisEmpire] > makePeace || atWarEmpire.DiplomacyModule.EmpireInDanger() == true))
            {
                Debug.Log(thisEmpireOpinions[atWarEmpire]);
                Debug.Log(makePeace);
                Debug.Log(EmpireInDanger());
                Debug.Log(atWarEmpire.DiplomacyModule.thisEmpireOpinions[thisEmpire]);
                Debug.Log(atWarEmpire.DiplomacyModule.EmpireInDanger());

                empiresToMakePeaceWith.Add(atWarEmpire);
            }

            // In here if too strong make peace with them by making them like you
            if (thisEmpire.WarModule.GetAllAllianceThreatRating(atWarEmpire) > atWarEmpire.WarModule.GetAllAllianceThreatRating(thisEmpire) + makePeace)
            {
                if (atWarEmpire.DiplomacyModule.thisEmpireOpinions[thisEmpire] < 0)
                {
                    if (empireToImproveRelations == null)
                    {
                        empireToImproveRelations = atWarEmpire;
                        amountToIncrease = atWarEmpire.DiplomacyModule.GetMakePeaceValue() - atWarEmpire.DiplomacyModule.thisEmpireOpinions[thisEmpire];
                    }
                    else
                    {
                        if (thisEmpire.WarModule.GetAllAllianceThreatRating(atWarEmpire) > thisEmpire.WarModule.GetAllAllianceThreatRating(empireToImproveRelations))
                        {
                            empireToImproveRelations = atWarEmpire;
                            amountToIncrease = atWarEmpire.DiplomacyModule.GetMakePeaceValue() - atWarEmpire.DiplomacyModule.thisEmpireOpinions[thisEmpire];
                        }
                    }
                }
            }
        }

        foreach (var makePeaceEmpire in empiresToMakePeaceWith)
        {
            thisEmpire.WarModule.MakePeace(makePeaceEmpire);
            makePeaceEmpire.WarModule.MakePeace(thisEmpire);
        }

        if (empireToImproveRelations == null)
        {
            foreach (var allEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
            {
                if (alliedEmpires.Count == 0) //If not already allied try to make an alliance
                {
                    if (empireToImproveRelations == null)
                    {
                        empireToImproveRelations = allEmpire;
                        amountToIncrease = empireToImproveRelations.DiplomacyModule.GetMakeAllianceValue() - empireToImproveRelations.DiplomacyModule.thisEmpireOpinions[thisEmpire];
                    }
                    else
                    {
                        if (empireToImproveRelations.DiplomacyModule.GetThisEmpireOpinion(thisEmpire) < allEmpire.DiplomacyModule.GetThisEmpireOpinion(thisEmpire)) // Get the empire with the one you have the highest opinion of.
                        {
                            empireToImproveRelations = allEmpire;
                            amountToIncrease = empireToImproveRelations.DiplomacyModule.GetMakeAllianceValue() - empireToImproveRelations.DiplomacyModule.thisEmpireOpinions[thisEmpire];
                        }
                    }

                }
            }
        }

        // If you are weaker and likely to get attacked then try to make alliance
        if (empireToImproveRelations != null)
        {
            //Try to ally with the most likely to ally empire
            if (gainedFavour == false)
            {
                empireToImproveRelations.DiplomacyModule.GainFavour(thisEmpire);
                if (amountToIncrease > 10 && EmpireInDanger() == true)
                {
                    if (thisEmpire.EconomyModule.GetCurrentMoney() * 0.5 > amountToIncrease/ moneyTimes)
                    {
                        GiftMoney(empireToImproveRelations, amountToIncrease * moneyTimes);
                    }
                }
            }
        }
    }
    /*
     * The below function is used to check if this empire should make an alliance with another empire and update war reason - values
     * Also will break alliances if the diplomacy reason has gone too low
     */ 
    public void DiplomacyCheck()
    {
        UpdateDiplomacy();

        foreach (var allEmpire in thisEmpire.WarModule.GetAllEmpiresInGame())
        {
            foreach (var atWarEmpire in thisEmpire.WarModule.GetAtWarEmpires())
            {
                if (allEmpire.GetEmpireNumber() == atWarEmpire.GetEmpireNumber())
                {
                    //The below will continuosly decrease the diplomacy module until it gets to -50
                    int warValue = thisEmpire.DiplomacyModule.GetDiplomacyReasonValue(atWarEmpire, "War");
                    if (warValue > -50)
                    {
                        thisEmpire.DiplomacyModule.ChangeValueInDiplomacyReasons(atWarEmpire, "War", warValue - rWarDecrease);
                    }
                    break;
                }
            }

            if (thisEmpire.DiplomacyModule.GetThisEmpireOpinion(allEmpire) > makeAllianceNumber && allEmpire.DiplomacyModule.GetThisEmpireOpinion(thisEmpire) > makeAllianceNumber) // If you both like each other
            {
                bool atWar = false;
                foreach (var atWarEmpire in thisEmpire.WarModule.GetAtWarEmpires())
                {
                    if (atWarEmpire == allEmpire)
                    {
                        atWar = true;
                    }
                }
                if (atWar == false)
                {
                    MakeAlliance(allEmpire);
                }
            }
            else
            {
                if (EmpireInDanger() == true)
                {
                    if (allEmpire.DiplomacyModule.GetThisEmpireOpinion(thisEmpire) > makeAllianceNumber)
                    {
                        MakeAlliance(allEmpire);
                    }
                }
            }

            List<EmpireClass> alliancesToBreak = new List<EmpireClass>();
            foreach (var alliedEmpire in alliedEmpires)
            {
                if (thisEmpire.DiplomacyModule.GetThisEmpireOpinion(alliedEmpire) < breakAllianceNumber && EmpireInDanger() == false)
                {
                    alliancesToBreak.Add(thisEmpire);
                }
            }

            foreach (var alliance in alliancesToBreak)
            {
                BreakAliiance(alliance);
            }
        } 
    }

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
    public int GetThisEmpireOpinion(EmpireClass _empire)
    {
        return thisEmpireOpinions[_empire];
    }

    /*
     * The below function will return all of the allied empires of this empire.
     */ 
    public List<EmpireClass> GetAlliedEmpires()
    {
        return alliedEmpires;
    }

    /*
     * The below function will return the boardering value.
     */ 
    public int GetRBoarderingValue()
    {
        return rBoardering;
    }

    /*
     * The below function can be used to get the make peace value of an empire
     */
    public int GetMakePeaceValue()
    {
        return makePeace;
    }

    /*
    * The below function can be used to get the make alliance value of an empire
    */
    public int GetMakeAllianceValue()
    {
        return makeAllianceNumber;
    }

    /*
    * This will return true or false depending on if there is a boardering empire that is stonger then you
    */
    public bool EmpireInDanger()
    {
        bool inDanger = false;

        //Check if there is an empire that dislikes you and is stronger then you
        foreach (var boarderingEmpire in thisEmpire.WarModule.GetBoarderingEmpires())
        {
            if (boarderingEmpire.WarModule.GetThreatRating(thisEmpire) < thisEmpire.WarModule.GetThreatRating(boarderingEmpire))
            {
                inDanger = true;
            }
        }
        return inDanger;
    }

    /*
     * The below function can be used to get a list of all the Empires that currently dislike this empire which are boardering it
     */
    public List<EmpireClass> EmpireDislikedBy()
    {
       List<EmpireClass> empiresToReturn = new List<EmpireClass>();
       foreach (var boarderingEmpire in thisEmpire.WarModule.GetBoarderingEmpires())
       {
            if (boarderingEmpire.DiplomacyModule.GetThisEmpireOpinion(thisEmpire) < 0)
            {
                empiresToReturn.Add(boarderingEmpire);
            }
       }
        return empiresToReturn;
    }
}

