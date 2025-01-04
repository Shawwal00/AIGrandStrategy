using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    // Start is called before the first frame update

    private Dictionary<int, Dictionary<string, int>> allTileData = new Dictionary<int, Dictionary<string, int>>();

    private void Awake()
    {
        allTileData[1] = new Dictionary<string, int>();
        allTileData[1]["Present"] = 25;
        allTileData[1]["Replenish"] = 5;

        allTileData[2] = new Dictionary<string, int>();
        allTileData[2]["Present"] = 25;
        allTileData[2]["Replenish"] = 5;

        allTileData[3] = new Dictionary<string, int>();
        allTileData[3]["Present"] = 25;
        allTileData[3]["Replenish"] = 5;

        allTileData[4] = new Dictionary<string, int>();
        allTileData[4]["Present"] = 25;
        allTileData[4]["Replenish"] = 5;

        allTileData[5] = new Dictionary<string, int>();
        allTileData[5]["Present"] = 25;
        allTileData[5]["Replenish"] = 5;

        allTileData[6] = new Dictionary<string, int>();
        allTileData[6]["Present"] = 25;
        allTileData[6]["Replenish"] = 5;

        allTileData[7] = new Dictionary<string, int>();
        allTileData[7]["Present"] = 25;
        allTileData[7]["Replenish"] = 5;

        allTileData[8] = new Dictionary<string, int>();
        allTileData[8]["Present"] = 25;
        allTileData[8]["Replenish"] = 5;

        allTileData[9] = new Dictionary<string, int>();
        allTileData[9]["Present"] = 25;
        allTileData[9]["Replenish"] = 5;

        allTileData[10] = new Dictionary<string, int>();
        allTileData[10]["Present"] = 25;
        allTileData[10]["Replenish"] = 5;

        allTileData[11] = new Dictionary<string, int>();
        allTileData[11]["Present"] = 25;
        allTileData[11]["Replenish"] = 5;

        allTileData[12] = new Dictionary<string, int>();
        allTileData[12]["Present"] = 25;
        allTileData[12]["Replenish"] = 5;

        allTileData[13] = new Dictionary<string, int>();
        allTileData[13]["Present"] = 25;
        allTileData[13]["Replenish"] = 5;

        allTileData[14] = new Dictionary<string, int>();
        allTileData[14]["Present"] = 25;
        allTileData[14]["Replenish"] = 5;

        allTileData[15] = new Dictionary<string, int>();
        allTileData[15]["Present"] = 25;
        allTileData[15]["Replenish"] = 5;

        allTileData[16] = new Dictionary<string, int>();
        allTileData[16]["Present"] = 25;
        allTileData[16]["Replenish"] = 5;

        allTileData[17] = new Dictionary<string, int>();
        allTileData[17]["Present"] = 25;
        allTileData[17]["Replenish"] = 5;

        allTileData[18] = new Dictionary<string, int>();
        allTileData[18]["Present"] = 25;
        allTileData[18]["Replenish"] = 5;

        allTileData[19] = new Dictionary<string, int>();
        allTileData[19]["Present"] = 25;
        allTileData[19]["Replenish"] = 5;

        allTileData[20] = new Dictionary<string, int>();
        allTileData[20]["Present"] = 25;
        allTileData[20]["Replenish"] = 5;

        allTileData[21] = new Dictionary<string, int>();
        allTileData[21]["Present"] = 25;
        allTileData[21]["Replenish"] = 5;

        allTileData[22] = new Dictionary<string, int>();
        allTileData[22]["Present"] = 25;
        allTileData[22]["Replenish"] = 5;

        allTileData[23] = new Dictionary<string, int>();
        allTileData[23]["Present"] = 25;
        allTileData[23]["Replenish"] = 5;

        allTileData[24] = new Dictionary<string, int>();
        allTileData[24]["Present"] = 25;
        allTileData[24]["Replenish"] = 5;

        allTileData[25] = new Dictionary<string, int>();
        allTileData[25]["Present"] = 25;
        allTileData[25]["Replenish"] = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Dictionary<int, Dictionary<string, int>> GetTileData()
    {
        return allTileData;
    }
}
