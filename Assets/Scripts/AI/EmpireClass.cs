using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is responsible for the indivigual empires that will be used.
 * It will contain all the parameters that the ai needs to function.
*/

public class EmpireClass : MonoBehaviour
{

    private GameObject GameManager;
    private MapBoard MapBoardScript;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        MapBoardScript = GameManager.GetComponent<MapBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //The below function is used when the AI captures a new tile
    public void ConquerTerritory()
    {

    }
}
