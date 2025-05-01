using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MapTile;

public class MapBoard : MonoBehaviour
{
  //Variables
  [SerializeField] public MapTile boardPiece;
  [SerializeField] public int boardLengthX;
  [SerializeField] public int boardLengthY;
  [SerializeField] public GameObject startLocation;

  [SerializeField] public List<Material> allMaterials;
  
  //Used in Other Scripts
  private List<MapTile>  allTilePieces;
  private Renderer boardRenderer;
  
  private MapTile copyBoardPiece;
  private int yFirstLimit;
  private int yLastLimit;

  [SerializeField] public float seed = 0; //Assign in Inspector
  
  private void Awake()
  { 
    allTilePieces = new List<MapTile>();
    startLocation.transform.position = new Vector3(0, 0, 0);
    boardRenderer = boardPiece.GetComponent<Renderer>();
  }

    /*
     * Creates the map board 
     */
    public void CreateMap()
    {
        int tileNumber = 1;
        for (int i = 0; i < boardLengthX; i++)
        {
            for (int j = 0; j < boardLengthY; j++)
            {
                copyBoardPiece = Instantiate(boardPiece, startLocation.transform.position, startLocation.transform.rotation);
                startLocation.transform.position = new Vector3(startLocation.transform.position.x, 
                startLocation.transform.position.y + boardRenderer.bounds.size.y, startLocation.transform.position.z );
                allTilePieces.Add(copyBoardPiece);
                copyBoardPiece.SetTileNumber(tileNumber);
                copyBoardPiece.SetTroopPresent((int)PerlinNoise(i, j, 5, 40));
                copyBoardPiece.SetTroopAdding((int)PerlinNoise(i, j, 1, 10));
                copyBoardPiece.SetCurrentPopulation((int)PerlinNoise(i, j, 30, 100));
                copyBoardPiece.SetAddingPopulation((int)PerlinNoise(i, j, 5, 20));
                copyBoardPiece.SetIncome((int)PerlinNoise(i, j, 15, 45));
                copyBoardPiece.SetAmeneties((int)PerlinNoise(i, j, 1, 7));
                copyBoardPiece.SetCorruptPopulation((int)PerlinNoise(i, j, 1, 7));

                float tileRandom = PerlinNoise(i, j, 1, 25);
                if (tileRandom/25 < 0.5)
                {
                    copyBoardPiece.thisTileType = TileType.None;
                }
                else if (tileRandom/25 >= 0.5 && tileRandom/25 < 0.6)
                {
                    copyBoardPiece.thisTileType = TileType.Plain;
                }
                else
                {
                    copyBoardPiece.thisTileType = TileType.Mine;
                }
                tileNumber++;
            }
            startLocation.transform.position = new Vector3(startLocation.transform.position.x + boardRenderer.bounds.size.x,
            startLocation.transform.position.y - boardRenderer.bounds.size.y * boardLengthY, 
            startLocation.transform.position.z);
        }
        AddAllConnections(allTilePieces, boardLengthX, boardLengthY);
    }

    /*
     * This adds all the connections for all the tiles
     * @param List<MapTile> completeList This is all of the tiles
     * @param int x This is the x amount of tiles
     * @param int y This is the y amount of tiles
     */
    public void AddAllConnections(List<MapTile> completeList, int x, int y)
    {
        yFirstLimit = y - 1;
        yLastLimit = completeList.Count - y;

        // This is working out the Y connections
        for (int i = 0; i < completeList.Count; i++)
        {
            if (i % y == y - 1)
            {
                completeList[i].AddConnectionTile(completeList[i - 1]);
            }
            else if (i % y == 0)
            {
                completeList[i].AddConnectionTile(completeList[i + 1]);
            }
            else
            {
                completeList[i].AddConnectionTile(completeList[i + 1]);
                completeList[i].AddConnectionTile(completeList[i - 1]);
            }
        }

        // Below this is working out the X Connections
        for (int i = 0; i <= yFirstLimit; i++)
        {
            completeList[i].AddConnectionTile(completeList[i + y]);
        }

        if (completeList.Count > y * 2)
        {
            for (int i = yFirstLimit + 1; i < yLastLimit; i++)
            {
                completeList[i].AddConnectionTile(completeList[i - y]);
                completeList[i].AddConnectionTile(completeList[i + y]);
            }
        }

        for (int i = yLastLimit; i < completeList.Count; i++)
        {
            completeList[i].AddConnectionTile(completeList[i - y]);
        }
    }

    public float PerlinNoise(int _x, int _y, int _lowest, int _highest)
    {
        float xCord = (float)(_x + seed) / boardLengthX;
        float yCord = (float)(_y + seed) / boardLengthY;

        float sample = Mathf.PerlinNoise(xCord , yCord);

        int total = _lowest + (int)(sample * (_highest - _lowest));
        seed += 1;
        return total;
    }
    
    /*
     * This returns a list of all the tiles
     * @return List<MapTile> allBoardPieces The list of all the tiles
     */ 
    public List<MapTile> ReturnTileList()
    {
        return allTilePieces;
    }

    /*
     * The below returns the board renderer
     * 
     */ 
    public Renderer ReturnRenderer()
    {
        return boardRenderer;
    }
}
