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
        allBuildingData["Fort"] = new Dictionary<string, int>(); // Can go on any tile
        allBuildingData["Fort"]["Price"] = 100;
        allBuildingData["Fort"]["Built"] = 0;

        allBuildingData["Mine"] = new Dictionary<string, int>(); // Specific on mine tiles
        allBuildingData["Mine"]["Price"] = 100;
        allBuildingData["Mine"]["Built"] = 0;

        allBuildingData["Barracks"] = new Dictionary<string, int>(); // Can go on plain tiles
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
