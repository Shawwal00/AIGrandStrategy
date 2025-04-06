using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
  private Renderer pieceRenderer;
  
  private MapTile copyBoardPiece;
  private int yFirstLimit;
  private int yLastLimit;
  
  private void Awake()
  { 
    allTilePieces = new List<MapTile>();
    startLocation.transform.position = new Vector3(0, 0, 0);
    pieceRenderer = boardPiece.GetComponent<Renderer>();
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
                startLocation.transform.position.y + pieceRenderer.bounds.size.y, startLocation.transform.position.z );
                allTilePieces.Add(copyBoardPiece);
                copyBoardPiece.SetTileNumber(tileNumber);

                tileNumber++;
            }
            startLocation.transform.position = new Vector3(startLocation.transform.position.x + pieceRenderer.bounds.size.x,
            startLocation.transform.position.y - pieceRenderer.bounds.size.y * boardLengthY, 
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

    
    /*
     * This returns a list of all the tiles
     * @return List<MapTile> allBoardPieces The list of all the tiles
     */ 
    public List<MapTile> returnTileList()
    {
        return allTilePieces;
    }
}
