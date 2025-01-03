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
  private List<MapTile>  allBoardPieces;
  private Renderer pieceRenderer;
  
  private MapTile copyBoardPiece;
  private int yFirstLimit;
  private int yLastLimit;
  
  private void Awake()
  { 
    allBoardPieces = new List<MapTile>();
    startLocation.transform.position = new Vector3(0, 0, 0);
    pieceRenderer = boardPiece.GetComponent<Renderer>();
  }
  
  //Creates the map board 
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
                allBoardPieces.Add(copyBoardPiece);
                copyBoardPiece.SetTileNumber(tileNumber);

                tileNumber++;
            }
            startLocation.transform.position = new Vector3(startLocation.transform.position.x + pieceRenderer.bounds.size.x,
            startLocation.transform.position.y - pieceRenderer.bounds.size.y * boardLengthY, 
            startLocation.transform.position.z);
        }
        AddAllConnections(allBoardPieces, boardLengthX, boardLengthY);
  }

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
      completeList[i].AddConnectionTile(completeList[i + x]);
    }

    if (completeList.Count > y * 2)
    {
      for (int i = yFirstLimit + 1; i < yLastLimit; i++)
      {
        completeList[i].AddConnectionTile(completeList[i - x]);
        completeList[i].AddConnectionTile(completeList[i + x]);
      }
    }

    for (int i = yLastLimit; i < completeList.Count; i++)
    {
      completeList[i].AddConnectionTile(completeList[i - x]);
    }
  }

    public List<MapTile> returnTileList()
    {
        return allBoardPieces;
    }
}
