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
        Debug.Log("Adding Empire");
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
           StartCoroutine(ConquerRegion());
        }
    }


    //The below function is used to make the AI's expand to another region.
    private IEnumerator ConquerRegion()
    {
        startAI = false;
        for (int i = 0; i < allAIEmpireClasses.Count; i++)
        {
            allAIEmpireClasses[i].ConquerTerritory();
            for (int j = 0; j < allAIEmpireClasses.Count; j++)
            {
                allAIEmpireClasses[j].SetAllTilesList(MapBoardScript.returnTileList(), allAIEmpireClasses[j].GetEmpireNumber());
            }
        }
        yield return new WaitForSeconds(0.1f);
        startAI = true;
    }

}
