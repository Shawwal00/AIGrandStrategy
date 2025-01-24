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

    //The below function will add an empire to the list 
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

    //The below function is used to make the AI's expand to another region.
    private IEnumerator MainLoop(EmpireClass _currentEmpire)
    {
        startAI = false;
        ConquerRegion(_currentEmpire);
        UpdateAllThreatRatings(_currentEmpire);
        EmpireClass warCheck = _currentEmpire.GoToWarCheck();
        if (warCheck != null)
        {
            warCheck.EmpireAtWarWith(_currentEmpire);
            _currentEmpire.EmpireAtWarWith(warCheck);
        }
        FightOtherEmpire(_currentEmpire);
        yield return new WaitForSeconds(0.1f);
        startAI = true;
    }

    //The below function will attempt to conquer a region on the map
    private void ConquerRegion(EmpireClass _currentEmpire)
    {
        _currentEmpire.ConquerTerritory();
        for (int j = 0; j < allAIEmpireClasses.Count; j++)
        {
            allAIEmpireClasses[j].SetAllTilesList(MapBoardScript.returnTileList());
        }
    }

    //The below function will update all the threat ratings of the empires
    private void UpdateAllThreatRatings(EmpireClass _currentEmpire)
    {
        foreach (var empire in _currentEmpire.GetBoarderingEmpires())
        {
            _currentEmpire.UpdateThreatRating(empire);
        }
    }

    //The below function is used to fight another enemy empire AI
    private void FightOtherEmpire(EmpireClass _currentEmpire)
    {
        if (_currentEmpire.GetAtWarEmpires().Count > 0) // Only occur if at war with an empire
        {
            foreach (var empire in _currentEmpire.GetAtWarEmpires())
            {
                if (empire.GetEmpireDefeated() == false)
                {
                    bool empireAlreadyDefeated = false;
                    if (_currentEmpire.GetDefeatedEmpires().Count > 0)
                    {
                        foreach (var defeatedEmpire in _currentEmpire.GetDefeatedEmpires())
                        {
                            if (defeatedEmpire == empire)
                            {
                                empireAlreadyDefeated = true;
                            }
                        }
                    }

                    if (empire.GetDefeatedEmpires().Count > 0)
                    {
                        foreach (var defeatedEmpire in empire.GetDefeatedEmpires())
                        {
                            if (defeatedEmpire == _currentEmpire)
                            {
                                empireAlreadyDefeated = true;
                            }
                        }
                    }

                    if (empireAlreadyDefeated == false)
                    {
                        Debug.Log("Empires Fighting");
                        Debug.Log(_currentEmpire.GetEmpireNumber());
                        Debug.Log(_currentEmpire.GetTroopNumber());
                        Debug.Log(empire.GetTroopNumber());
                        int newCurrentTroopNumber = _currentEmpire.GetTroopNumber() - empire.GetTroopNumber();
                        int otherEmpireTroopNumber = empire.GetTroopNumber() - _currentEmpire.GetTroopNumber();

                        if (newCurrentTroopNumber > otherEmpireTroopNumber)
                        {
                            _currentEmpire.AddToDefeatedEmpires(empire);
                            empire.SetEmpireDefeatedTrue(_currentEmpire);
                        }
                        else
                        {
                            empire.AddToDefeatedEmpires(_currentEmpire);
                            _currentEmpire.SetEmpireDefeatedTrue(empire);
                        }
                        _currentEmpire.SetTroopNumber(newCurrentTroopNumber);
                        empire.SetTroopNumber(otherEmpireTroopNumber);
                    }
                }
            }
        }
    }

}
