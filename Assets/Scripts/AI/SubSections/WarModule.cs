using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Dictionary<EmpireClass, int> threatRatings = new Dictionary<EmpireClass, int>(); // 1 is a threat // -1 is not a threat

    private bool empireDefeated = false;
    private float armyDestroyedTime = 0; // How long it has been since the enemy army was destroyed
    private EmpireClass empireThatDefeatedYou; // This is the empire that defeated you

    private int troopNumber = 0; 
    private float updateTroopNumberTime = 0;

    private void Awake()
    {
        //Gameobject
        GameManager = GameObject.Find("GameManager");

        //Scripts
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();

        //Lists
        boarderingEmpires = new List<EmpireClass>();
        atWarEmpires = new List<EmpireClass>();
        empiresDefeatedInBattle = new List<EmpireClass>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllTroopCount();

        //The below is if you have been defeated then check to see if your army can be restored
        if (empireDefeated == true)
        {
            armyDestroyedTime += Time.deltaTime;
            if (armyDestroyedTime > 2)
            {
                empireDefeated = false;
                armyDestroyedTime = 0;
                empireThatDefeatedYou.WarModule.RemoveEmpireFromeDefeatedList(thisEmpire);
            }
        }
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
     * This gets all the tiles owned by the empire and adds to the total empire troop count.
     */
    private void UpdateAllTroopCount()
    {
        updateTroopNumberTime += Time.deltaTime;
        if (thisEmpire.ReturnOwnedTiles().Count > 0)
        {
            if (updateTroopNumberTime > 1)
            {
                for (int i = 0; i < thisEmpire.ReturnOwnedTiles().Count; i++)
                {
                    // Debug.Log(ownedTiles[i].GetTroopAdding());
                    AddToTroopNumber(thisEmpire.ReturnOwnedTiles()[i].GetTroopAdding());
                }
                updateTroopNumberTime = 0;
            }
        }
    }

    /*
     * The below function is used when the AI captures a new tile
     */
    public void ConquerTerritory()
    {
       thisEmpire.GetExpandingTilesOfTile();
        int randomNumber = Random.Range(0, thisEmpire.ReturnExpandingTiles().Count);
        if (thisEmpire.ReturnExpandingTiles().Count > 0)
        {
            MapTile lowestTile = null;

            foreach (MapTile tile in thisEmpire.ReturnExpandingTiles())
            {
                if (lowestTile == null)
                {
                    if (tile.GetOwner() == 0 || SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner()).WarModule.GetEmpireDefeated() == true)
                    {
                        lowestTile = tile;
                    }
                }
                else
                {
                    if (tile.GetOwner() == 0 || SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner()).WarModule.GetEmpireDefeated() == true)
                    {
                        if (tile.GetTroopPresent() < lowestTile.GetTroopPresent())
                        {
                            lowestTile = tile;
                        }
                    }
                }
            }
            if (lowestTile != null && lowestTile.GetTroopPresent() < troopNumber)
            {
                lowestTile.SetOwner(thisEmpire.GetEmpireNumber());
                lowestTile.SetTroopPresent(10);
                SetTroopNumber(troopNumber - lowestTile.GetTroopPresent());

                List<MapTile> mapTiles = lowestTile.GetAllConnectedTiles();
                foreach (MapTile tile in mapTiles)
                {
                    if (tile.GetOwner() != 0 && tile.GetOwner() != thisEmpire.GetEmpireNumber())
                    {
                        bool empireFound = false;
                        foreach (var empire in boarderingEmpires)
                        {
                            if (empire.GetEmpireNumber() == tile.GetOwner())
                            {
                                empireFound = true;
                            }
                        }
                        if (empireFound == false)
                        {
                            EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner());
                            boarderingEmpires.Add(otherEmpire);
                            UpdateThreatRating(otherEmpire);
                            otherEmpire.WarModule.OtherEmpireConquredNewTile(thisEmpire);
                        }
                        break;
                    }
                }
            }
        }
    }


    /*
     * This function is called when another empire has been defeated and all refrences to it should be destroyed
     * @param EmpireClass _deadEmpire This is the empire that has died
     */
    public void OtherEmpireDied(EmpireClass _deadEmpire)
    {
        if (empiresDefeatedInBattle.Contains(_deadEmpire))
        {
            empiresDefeatedInBattle.Remove(_deadEmpire);
        }
        if (atWarEmpires.Contains(_deadEmpire))
        {
            atWarEmpires.Remove(_deadEmpire);
        }
        if (boarderingEmpires.Contains(_deadEmpire))
        {
            boarderingEmpires.Remove(_deadEmpire);
        }
    }

    /*
     * The below function will occur if another empire has conqured a tile on the map
     * @param EmpireClass _newBoarderingEmpire The newly encountered empire
     */
    public void OtherEmpireConquredNewTile(EmpireClass _newBoarderingEmpire)
    {
        boarderingEmpires.Add(_newBoarderingEmpire);
        UpdateThreatRating(_newBoarderingEmpire);
    }

    /*
     * The below function occurs when an empire has declared war on war
     * @param EmpireClass _empireThatDeclaredWar The empire that has declared war on you
     */
    public void EmpireAtWarWith(EmpireClass _empireThatDeclaredWar)
    {
        atWarEmpires.Add(_empireThatDeclaredWar);
    }

    /*
     * The below function will update the threat rating of another empire
     * @param EmpireClass _otherEmpire The empires whos threat rating you are updating
     */
    public void UpdateThreatRating(EmpireClass _otherEmpire)
    {
        int newThreatRating = 0;
        if (_otherEmpire.WarModule.troopNumber * 1.1 > troopNumber)
        {
            newThreatRating = 1;
        }
        else
        {
            newThreatRating = -1;
        }
        threatRatings[_otherEmpire] = newThreatRating;
    }

    /*
     * The below function is used to check if the AI should go to war with a neighbouring faction
     */
    public EmpireClass GoToWarCheck()
    {
        if (troopNumber > 100)
        {
            foreach (var empire in boarderingEmpires)
            {
                if (threatRatings[empire] == -1)
                {
                    return empire;
                }
            }
        }
        return null;
    }

    /*
     * This function is used to make peace with another empire
     */ 
    public void MakePeace()
    {
        foreach (var otherEmpire in atWarEmpires)
        {
            if (otherEmpire.WarModule.troopNumber > troopNumber)
            {
                //Attempt to make peace they have more troops.
            }
        }
    }

    /*
     * Set the empireDefeated variable to true when the ai has lost a fight and set the empire that has defeated you.
     * @param EmpireClass _newEmpireThatDefeatedYou This is the empire that defeated you in battle
     */
    public void SetEmpireDefeatedTrue(EmpireClass _newEmpireThatDefeatedYou)
    {
        empireDefeated = true;
        empireThatDefeatedYou = _newEmpireThatDefeatedYou;
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
     * The below function will return all the defeated Empires
     * @return List<EmpireClass> empiresDefeatedInBattle These are the empires you have defeated
     */
    public List<EmpireClass> GetDefeatedEmpires()
    {
        return empiresDefeatedInBattle;
    }

    /*
     * Removes the empire from the defeated list.
     * @param EmpireClass _empireToRemove This is that empire that will be removed
     */
    public void RemoveEmpireFromeDefeatedList(EmpireClass _empireToRemove)
    {
        empiresDefeatedInBattle.Remove(_empireToRemove);
    }

    /*
     * The below function is for when you have defeated an empire in battle allowing you to occupy thier territory
     * @param EmpireClass _defeatedEmpire This is the empire that you have defeated in battle
     */
    public void AddToDefeatedEmpires(EmpireClass _defeatedEmpire)
    {
        empiresDefeatedInBattle.Add(_defeatedEmpire);
    }

    /*
     * This will add to the empires troop number
     * @param int _addTroopNumber The amount of new troops you will be adding
     */
    public void AddToTroopNumber(int _addTroopNumber)
    {
        troopNumber += _addTroopNumber;
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
     * Returns if the empire has been defeated
     * @return bool empireDefeated This will return if you have been defeated in battle or not
     */
    public bool GetEmpireDefeated()
    {
        return empireDefeated;
    }
}
