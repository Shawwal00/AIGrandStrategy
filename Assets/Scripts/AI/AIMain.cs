using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script handles the all the overall functions.
*/

public class AIMain : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard MapBoardScript;

    private List<EmpireClass> allAIEmpireClasses;
    private bool startAI = false;

    private void Awake()
    {
        allAIEmpireClasses = new List<EmpireClass>();
    }

    /*
     * The below function will add an empire to the list 
     * @param EmpireClass _empireToAdd This is the empire yo uare adding to the list
     */
    public void AddEmpireToList(EmpireClass _empireToAdd)
    {
        allAIEmpireClasses.Add(_empireToAdd);
        if (startAI == false)
        {
            startAI = true;
        }
    }

    private void Update()
    {
        if (startAI == true)
        {
            for (int i = 0; i < allAIEmpireClasses.Count; i++)
            {
                StartCoroutine(MainLoop(allAIEmpireClasses[i]));
            }
        }
    }

    /*
     * The below function is used to handle the main loop of the AI.
     * @param EmpireClass _currentEmpire This is the empire currently 'taking its turn' 
     */
    private IEnumerator MainLoop(EmpireClass _currentEmpire)
    {
        startAI = false;
        _currentEmpire.EconomyModule.UpdateEmpireMoney();
        _currentEmpire.WarModule.UpdateThreatReasons();
        _currentEmpire.DiplomacyModule.DiplomacyCheck();
        ConquerRegion(_currentEmpire);
       // UpdateAllThreatRatings(_currentEmpire);
        EmpireClass warCheck = _currentEmpire.WarModule.GoToWarCheck();
        if (warCheck != null)
        {
            _currentEmpire.WarModule.EmpireAtWarWith(warCheck);
            warCheck.WarModule.EmpireAtWarWith(_currentEmpire);
        }
        FightOtherEmpire(_currentEmpire);
        yield return new WaitForSeconds(0.1f);
        startAI = true;
    }

    /*
     * The below function will attempt to conquer a region on the map
     * @param EmpireClass _currentEmpire This is the empire conquering
     */
    private void ConquerRegion(EmpireClass _currentEmpire)
    {
        _currentEmpire.WarModule.ConquerTerritory();
        for (int j = 0; j < allAIEmpireClasses.Count; j++)
        {
            allAIEmpireClasses[j].SetAllTilesList(MapBoardScript.returnTileList());
        }
    }

    /*
     * The below function will update all the threat ratings of the empires
     * @param EmpireClass _currentEmpire This is the empire which is updating its threat ratings
     */
    //private void UpdateAllThreatRatings(EmpireClass _currentEmpire)
    //{
    //   foreach (var empire in _currentEmpire.WarModule.GetBoarderingEmpires())
    //    {
    //        _currentEmpire.WarModule.UpdateThreatRating(empire);
    //    }
    //}

   /*
   * The below function will destroy an empire and let the other empires know
   * @param EmpireClass _destroyedEmpire This is the empire which has been destroyed
   */
    public void EmpireDestroyed(EmpireClass _destroyedEmpire)
    {
        foreach (var empire in allAIEmpireClasses)
        {
            empire.WarModule.OtherEmpireDied(_destroyedEmpire);
        }

        allAIEmpireClasses.Remove(_destroyedEmpire);
    }

    /*
     * The below function is used to fight another enemy empire AI
     * param EmpireClass _currentEmpire This is the empire who is attacking
     */
    private void FightOtherEmpire(EmpireClass _currentEmpire)
    {
        if (_currentEmpire.WarModule.GetAtWarEmpires().Count > 0) // Only occur if at war with an empire
        {
            foreach (var empire in _currentEmpire.WarModule.GetAtWarEmpires())
            {
                if (empire.WarModule.GetEmpireDefeated() == false)
                {
                    bool empireAlreadyDefeated = false;
                    if (_currentEmpire.WarModule.GetDefeatedEmpires().Count > 0)
                    {
                        foreach (var defeatedEmpire in _currentEmpire.WarModule.GetDefeatedEmpires())
                        {
                            if (defeatedEmpire == empire)
                            {
                                empireAlreadyDefeated = true;
                            }
                        }
                    }

                    if (empire.WarModule.GetDefeatedEmpires().Count > 0)
                    {
                        foreach (var defeatedEmpire in empire.WarModule.GetDefeatedEmpires())
                        {
                            if (defeatedEmpire == _currentEmpire)
                            {
                                empireAlreadyDefeated = true;
                            }
                        }
                    }

                    if (empireAlreadyDefeated == false)
                    {
                        //Debug.Log("Empires Fighting");
                        //Debug.Log(_currentEmpire.GetEmpireNumber());
                        //Debug.Log(_currentEmpire.GetTroopNumber());
                        //Debug.Log(empire.GetTroopNumber());
                        int newCurrentTroopNumber = _currentEmpire.WarModule.GetTroopNumber() - empire.WarModule.GetTroopNumber();
                        int otherEmpireTroopNumber = empire.WarModule.GetTroopNumber() - _currentEmpire.WarModule.GetTroopNumber();

                        if (newCurrentTroopNumber > otherEmpireTroopNumber)
                        {
                            _currentEmpire.WarModule.AddToDefeatedEmpires(empire);
                            empire.WarModule.SetEmpireDefeatedTrue(_currentEmpire);
                        }
                        else
                        {
                            empire.WarModule.AddToDefeatedEmpires(_currentEmpire);
                            _currentEmpire.WarModule.SetEmpireDefeatedTrue(empire);
                        }
                        _currentEmpire.WarModule.SetTroopNumber(newCurrentTroopNumber);
                        empire.WarModule.SetTroopNumber(otherEmpireTroopNumber);
                    }
                }
            }
        }
    }

    /*
     * The below function can be used to return a list of all of the empires
     */ 
    public List<EmpireClass> ReturnAllEmpiresInGame()
    {
        return allAIEmpireClasses;
    }

}
