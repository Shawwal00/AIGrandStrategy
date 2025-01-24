using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * This script is responsible for the indivigual empires that will be used.
 * It will contain all the parameters that the ai needs to function.
*/

public class EmpireClass : MonoBehaviour
{

    private GameObject GameManager;
    private MapBoard MapBoardScript;
    private SetUpEmpires SetUpEmpires;
    private AIMain aiMain;

    private List<MapTile> allTilesList; //All the tiles on the board
    private List<MapTile> ownedTiles; // All the tiles owned by this empire
    private List<MapTile> expandingTiles; // All the tiles that this empire can expand to

    private int empireNumber = 0;
    private bool empireDefeated = false;
    private float armyDestroyedTime = 0;
    private EmpireClass empireThatDefeatedYou;

    private int troopNumber = 0;
    private float updateTroopNumberTime = 0;

    // private bool boarderingEmpire = false;

    private List<EmpireClass> boarderingEmpires;
    private List<EmpireClass> atWarEmpires;
    private List<EmpireClass> empiresDefeatedInBattle;
    private Dictionary<EmpireClass, int> threatRatings = new Dictionary<EmpireClass, int>(); // 1 is a threat // -1 is not a threat

    private Color empireColor = Color.white;
    private bool alive = true;

    private void Awake()
    {
        allTilesList = new List<MapTile>();
        ownedTiles = new List<MapTile>();
        expandingTiles = new List<MapTile>();
        boarderingEmpires = new List<EmpireClass>();
        GameManager = GameObject.Find("GameManager");
        MapBoardScript = GameManager.GetComponent<MapBoard>();
        SetUpEmpires = GameManager.GetComponent<SetUpEmpires>();
        aiMain = GameManager.GetComponent<AIMain>();
        atWarEmpires = new List<EmpireClass>();
        empiresDefeatedInBattle = new List<EmpireClass>();
    }

    private void Update()
    {
        UpdateAllTroopCount();

        if (empireDefeated == true)
        {
            armyDestroyedTime += Time.deltaTime;
            if (armyDestroyedTime > 2)
            {
                empireDefeated = false;
                armyDestroyedTime = 0;
                empireThatDefeatedYou.RemoveEmpireFromeDefeatedList(this);
            }
        }
    }

    //This gets all the tiles owned by the empire and adds to the total empire troop count.
    private void UpdateAllTroopCount()
    {
        updateTroopNumberTime += Time.deltaTime;
        if (ownedTiles.Count > 0)
        {
            if (updateTroopNumberTime > 1)
            {
                for (int i = 0; i < ownedTiles.Count; i++)
                {
                    // Debug.Log(ownedTiles[i].GetTroopAdding());
                    AddToTroopNumber(ownedTiles[i].GetTroopAdding());
                }
                updateTroopNumberTime = 0;
            }
        }
    }

    //The below function is used when the AI captures a new tile
    public void ConquerTerritory()
    {
        GetExpandingTilesOfTile();
        int randomNumber = Random.Range(0, expandingTiles.Count);
        if (expandingTiles.Count > 0)
        {
            MapTile lowestTile = null;

            foreach (MapTile tile in expandingTiles)
            {
                if (lowestTile == null)
                {
                    if (tile.GetOwner() == 0 || SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner()).GetEmpireDefeated() == true)
                    {
                        lowestTile = tile;
                    }
                }
                else
                {
                    if (tile.GetOwner() == 0 || SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner()).GetEmpireDefeated() == true)
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
                lowestTile.SetOwner(empireNumber);
                lowestTile.SetTroopPresent(10);
                SetTroopNumber(troopNumber - lowestTile.GetTroopPresent());

                List<MapTile> mapTiles = lowestTile.GetAllConnectedTiles();
                foreach (MapTile tile in mapTiles)
                {
                    if (tile.GetOwner() != 0 && tile.GetOwner() != empireNumber)
                    {
                        bool empireFound = false;
                        foreach (var empire in boarderingEmpires)
                        {
                            if (empire.empireNumber == tile.GetOwner())
                            {
                                empireFound = true;
                            }
                        }
                        if (empireFound == false)
                        {
                            EmpireClass otherEmpire = SetUpEmpires.GetSpecificEmpireClassBasedOnOwner(tile.GetOwner());
                            boarderingEmpires.Add(otherEmpire);
                            UpdateThreatRating(otherEmpire);
                            otherEmpire.OtherEmpireConquredNewTile(this);
                        }
                        break;
                    }
                }
            }
        }
    }

    // This function is called when another empire has been defeated and all refrences to it should be destroyed
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

    //The below function will occur if another empire has conqured a tile on the map
    public void OtherEmpireConquredNewTile(EmpireClass _newBoarderingEmpire)
    {
        boarderingEmpires.Add(_newBoarderingEmpire);
        UpdateThreatRating(_newBoarderingEmpire);
    }

    //The below function occurs when an empire has declared war on war
    public void EmpireAtWarWith(EmpireClass _empireThatDeclaredWar)
    {
        atWarEmpires.Add(_empireThatDeclaredWar);
    }

    //The below function will update the threat rating of another empire
    public void UpdateThreatRating(EmpireClass _otherEmpire)
    {
        int newThreatRating = 0;
        if (_otherEmpire.troopNumber * 1.1 > troopNumber)
        {
            newThreatRating = 1;
        }
        else
        {
            newThreatRating = -1;
        }
        threatRatings[_otherEmpire] = newThreatRating;
    }

    //The below function is used to check if the AI should go to war with a neighbouring faction
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

    public void MakePeace()
    {
        foreach (var otherEmpire in atWarEmpires)
        {
            if (otherEmpire.troopNumber > troopNumber)
            {
                //Attempt to make peace they have more troops.
            }
        }
    }

    //The below function is used to get a list of all the tiles when the map is set up.
    public void SetAllTilesList(List<MapTile> _newTileList)
    {
        allTilesList = _newTileList;
        GetOwnedTiles();
    }

    //The below function is used to get tiles that are owned by this empire specifically.
    public void GetOwnedTiles()
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
            aiMain.EmpireDestroyed(this);
        }
    }

    //The below function will get the tiles that are adjacent to a tile and put them into a list
    public void GetExpandingTilesOfTile()
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

    // This function will set the new empire number
    public void SetEmpireNumber(int _newEmpireNumber)
    {
        empireNumber = _newEmpireNumber;
    }

    //Return the empire number
    public int GetEmpireNumber()
    {
        return empireNumber;
    }

    //This will add to the empires troop number
    public void AddToTroopNumber(int _addTroopNumber)
    {
        troopNumber += _addTroopNumber;
    }

    //The below will set the empires troop number
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

    public int GetTroopNumber()
    { 
        return troopNumber; 
    }

    //The below function will return the boardering empire value
    public List<EmpireClass> GetBoarderingEmpires()
    {
        return boarderingEmpires;
    }

    //The below function will return all the at war Empires
    public List<EmpireClass> GetAtWarEmpires()
    {
        return atWarEmpires;
    }

    //The below function will return all the defeated Empires
    public List<EmpireClass> GetDefeatedEmpires()
    {
        return empiresDefeatedInBattle;
    }

    // Removes the empire from the defeated list.
    public void RemoveEmpireFromeDefeatedList(EmpireClass _empireToRemove)
    {
        empiresDefeatedInBattle.Remove(_empireToRemove);
    }

    // The below function is for when you have defeated an empire in battle allowing you to occupy thier territory
    public void AddToDefeatedEmpires(EmpireClass _defeatedEmpire)
    {
        empiresDefeatedInBattle.Add(_defeatedEmpire);
    }

    //Set the empireDefeated variable to true when the ai has lost a fight and set the empire that has defeated you.
    public void SetEmpireDefeatedTrue(EmpireClass _newEmpireThatDefeatedYou)
    {
        empireDefeated = true;
        empireThatDefeatedYou = _newEmpireThatDefeatedYou;
    }

    //Returns if the empire has been defeated
    public bool GetEmpireDefeated()
    {
        return empireDefeated;
    }

    //The below function will set the empire color for the tiles
    public void SetEmpireColor(Color newEmpireColor)
    {
        empireColor = newEmpireColor;
    }

    //The below function will return the empire color used for the tiles
    public Color GetEmpireColor()
    {
        return empireColor;
    }
}
