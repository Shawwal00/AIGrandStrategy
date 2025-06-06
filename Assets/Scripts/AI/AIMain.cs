using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
 * This script handles the all the overall functions fo the ai empires.
*/

public class AIMain : MonoBehaviour
{
    //Scripts
    [SerializeField] public MapBoard MapBoardScript;

    private List<EmpireClass> allAIEmpireClasses;
    private bool startAI = false;
    private bool inForLoop = false;
    private bool inFunction = false;
    private List<EmpireClass> toDestroy;

    private void Awake()
    {
        allAIEmpireClasses = new List<EmpireClass>();
        toDestroy = new List<EmpireClass>();
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
        StartCoroutine(StartLoop());
    }

    /*
     * This stars the main for loop for the ai empires
     */ 
    private IEnumerator StartLoop()
    {
        if (startAI == true && inForLoop == false)
        {
            inForLoop = true;
            foreach (var empire in allAIEmpireClasses)
            {
                while (startAI == false)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                if (startAI == true)
                {
                    StartCoroutine(MainLoop(empire));
                }
            }

            foreach (var destroyedEmpire in toDestroy)
            {
                foreach (var empire in allAIEmpireClasses)
                {
                    empire.WarModule.OtherEmpireDied(destroyedEmpire);
                }

                allAIEmpireClasses.Remove(destroyedEmpire);
            }
            toDestroy.Clear();
            inForLoop = false;
        }
    }

    /*
     * The below function is used to handle the main loop of the AI.
     * @param EmpireClass _currentEmpire This is the empire currently 'taking its turn' 
     */
    private IEnumerator MainLoop(EmpireClass _currentEmpire)
    {
        if (_currentEmpire.GetDestoryed() == false)
        {
            startAI = false;
            inFunction = true;
            _currentEmpire.WarModule.UpdateAllTroopCount();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.MigratePopulation();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.InternalModule.UpdateInternals();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.EconomyModule.UpdateEmpireMoney();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.WarModule.UpdateThreatReasons();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.DiplomacyModule.DiplomacyCheck();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            _currentEmpire.EconomyModule.BuildBuilding();
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            inFunction = true;
            StartCoroutine(_currentEmpire.WarModule.ConquerTerritory());
            while (inFunction == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            EmpireClass warCheck = _currentEmpire.WarModule.GoToWarCheck();
            if (warCheck != null && warCheck != _currentEmpire)
            {
                _currentEmpire.WarModule.EmpireAtWarWith(warCheck);
                warCheck.WarModule.EmpireAtWarWith(_currentEmpire);
            }
            yield return new WaitForSeconds(0.1f);
            startAI = true;
        }
    }

    /*
     * This is to let the main loop know that the current function has been finished.
     * @param bool _value The new value to set inFunction to
     */ 
    public void setInFunctionToFalse(bool _value)
    {
        inFunction = _value;
    }

   /*
   * The below function will destroy an empire and let the other empires know
   * @param EmpireClass _destroyedEmpire This is the empire which has been destroyed
   */
    public void EmpireDestroyed(EmpireClass _destroyedEmpire)
    {
        toDestroy.Add(_destroyedEmpire);
    }

    /*
     * The below function can be used to return a list of all of the empires
     */ 
    public List<EmpireClass> GetAllEmpiresInGame()
    {
        return allAIEmpireClasses;
    }

    /*
     * The below function is used to change the empire speed to normal
     */ 
    public void ChangeSpeedNormal()
    {
        foreach (EmpireClass empire in allAIEmpireClasses)
        {
            empire.SetSpeedNormal();
        }
    }

    /*
    * The below function is used to change the empire speed to slow
    */
    public void ChangeSpeedSlow()
    {
        foreach (EmpireClass empire in allAIEmpireClasses)
        {
            empire.SetSpeedSlow();
        }
    }

    /*
    * The below function is used to change the empire speed to fast
    */
    public void ChangeSpeedFast()
    {
        foreach (EmpireClass empire in allAIEmpireClasses)
        {
            empire.SetSpeedFast();
        }
    }

}
