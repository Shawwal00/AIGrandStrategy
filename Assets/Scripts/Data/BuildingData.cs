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

    /*
     * This changes the data of the tile so that one of the buildings is now set to owned.
     * @param string _buildingType This is what building ownership you are changing
     * @param int _owned This is what the the new value for the build will be
     */
    public void ChangeDataOwned(string _buildingType, int _owned)
    {
        allBuildingData[_buildingType]["Built"] = _owned;
    }

    /*
     * This get the Built of a specific building.
     * @param string _buildingType This is the specifc buildType for which you are getting the data for
     * @return int allBuildingData[_buildingType]["Built"] This is if the building has been built on this tile or not
     */
    public int GetBuildingDataOwned(string _buildingType)
    {
        return allBuildingData[_buildingType]["Built"];
    }

    /*
    * This get the Built of a specific building.
    * @param string _buildingType This is the specifc buildType for which you are getting the data for
    * @return int allBuildingData[_buildingType]["Price"] This is the price to build this building
    */
    public int GetBuildingDataPrice(string _buildingType)
    {
        return allBuildingData[_buildingType]["Price"];
    }
}
