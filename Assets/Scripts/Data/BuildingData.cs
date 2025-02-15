using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script handles the buillding data of the tiles.
*/


public class BuildingData : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, int>> allBuildingData = new Dictionary<string, Dictionary<string, int>>();

    private void Awake()
    {
        allBuildingData["Fort"] = new Dictionary<string, int>();
        allBuildingData["Fort"]["Price"] = 100;
        allBuildingData["Fort"]["Built"] = 0;

        allBuildingData["Market"] = new Dictionary<string, int>();
        allBuildingData["Market"]["Price"] = 100;
        allBuildingData["Market"]["Built"] = 0;

        allBuildingData["Barracks"] = new Dictionary<string, int>();
        allBuildingData["Barracks"]["Price"] = 100;
        allBuildingData["Barracks"]["Built"] = 0;
    }

    public void ChangeDataOwned(string _buildingType, int _owned)
    {
        allBuildingData[_buildingType]["Built"] = _owned;
    }

    public int GetBuiildingDataOwned(string _buildingType)
    {
        return allBuildingData[_buildingType]["Built"];
    }
}
