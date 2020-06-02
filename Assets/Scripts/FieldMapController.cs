using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldMapController : MonoBehaviour
{
    public int size = Constants.MAP_SIZE;
    public GameObject CellPrefab = null;
    private GameObject[,] mapArr;
    private GameObject[] shipArr;

    // Start is called before the first frame update
    void Start()
    {
        // Init map
        mapArr = new GameObject[Constants.MAP_SIZE, Constants.MAP_SIZE];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                GameObject cell = Instantiate(
                    CellPrefab,
                    new Vector3(
                        col * Constants.CELL_SIZE + Constants.LEFT_OFFSET,
                        Constants.CELL_ELEVATION,
                        row * Constants.CELL_SIZE + Constants.BOTTOM_OFFSET
                    ),
                    CellPrefab.transform.rotation
                );
                cell.GetComponent<CellController>().row = row;
                cell.GetComponent<CellController>().col = col;
                mapArr[row, col] = cell;
            }
        }
        shipArr = new GameObject[Constants.SHIP_AMOUNT];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2[] GetShipSafeArea(GameObject ship)
    {
        ShipController shipController = ship.GetComponent<ShipController>();
        Vector2[] shipCells = GetCellsInShip(shipController);
        Vector2[] surroundedCells = new Vector2[shipController.length * 9];
        int i = 0;
        foreach (Vector2 cell in shipCells)
        {
            foreach (int offsetRow in Enumerable.Range(1, 3))
            {
                foreach (int offsetCol in Enumerable.Range(1, 3))
                {
                    surroundedCells[i] = cell + new Vector2(offsetRow - 2, offsetCol - 2);
                    i++;
                }
            }
        }
        // TODO: check if true
        Vector2[] finalSurroundedCells = new HashSet<Vector2>(surroundedCells).ToArray();
        return finalSurroundedCells;
    }

    Vector2[] GetCellsInShip(ShipController shipController)
    {
        CellController rootCell = shipController.root.GetComponent<CellController>();
        Vector2 rootPos = new Vector2(rootCell.row, rootCell.col);
        Vector2 tailPos = rootPos + new Vector2(
            shipController.direction.deltaVertical * (shipController.length - 1),
            shipController.direction.deltaHorizontal * (shipController.length - 1)
        );
        Vector2[] shipCells = new Vector2[shipController.length];
        int i = 0;
        int maxRow = Math.Max((int)rootPos.x, (int)tailPos.x);
        int minRow = Math.Min((int)rootPos.x, (int)tailPos.x);
        int maxCol = Math.Max((int)rootPos.y, (int)tailPos.y);
        int minCol = Math.Min((int)rootPos.y, (int)tailPos.y);
        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                shipCells[i] = new Vector2(row, col);
                i++;
            }
        }
        return shipCells;
    }

    int[,] GetNumericMap()
    {
        int[,] numMapArr = new int[Constants.MAP_SIZE, Constants.MAP_SIZE];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                numMapArr[row, col] = -1;
                mapArr[row, col].GetComponent<CellController>().ChangeStatus(CellStatus.NORMAL);
            }
        }
        foreach (GameObject ship in shipArr)
        {
            if (ship == null) continue;
            ShipController shipController = ship.GetComponent<ShipController>();
            Vector2[] shipCells = GetCellsInShip(shipController);
            foreach (Vector2 cell in shipCells)
            {
                numMapArr[(int)cell.x, (int)cell.y] = shipController.id;
            }
        }
        return numMapArr;
    }

    bool CheckValidCellCord(int row, int col)
    {
        if (row < 0 || row > Constants.MAP_SIZE - 1 || col < 0 || col > Constants.MAP_SIZE - 1)
        {
            return false;
        }
        return true;
    }

    public bool CheckValidShipPos(GameObject ship, Vector2 position)
    {
        ResetCellStatus();
        ShipController shipController = ship.GetComponent<ShipController>();
        if (!CheckValidCellCord((int)position.x, (int)position.y))
        {
            return false;
        }
        shipController.root = mapArr[(int) position.x, (int) position.y];
        int[,] numMapArr = GetNumericMap();
        Vector2[] safeArea = GetShipSafeArea(ship);
        Vector2[] shipCells = GetCellsInShip(shipController);
        bool isValid = true;
        foreach (Vector2 cell in safeArea)
        {
            if (!CheckValidCellCord((int)cell.x, (int)cell.y))
            {
                bool cellInShip = false;
                foreach (Vector2 shipCell in shipCells)
                {
                    if (shipCell.Equals(cell))
                    {
                        cellInShip = true;
                    }
                }
                if (cellInShip)
                {
                    isValid = false;
                }
            }
            else
            {
                if (numMapArr[(int)cell.x, (int)cell.y] != -1)
                {
                    isValid = false;
                }
            }
        }
        // Change color
        foreach (Vector2 cell in safeArea)
        {
            if (CheckValidCellCord((int)cell.x, (int)cell.y))
            {
                if (isValid)
                {
                    mapArr[(int)cell.x, (int)cell.y].GetComponent<CellController>().ChangeStatus(CellStatus.VALID);
                }
                else
                {
                    mapArr[(int)cell.x, (int)cell.y].GetComponent<CellController>().ChangeStatus(CellStatus.INVALID);
                }
            }
        }
            return isValid;
    }

    public Vector2 GetCellPosFromShipPos(Vector3 shipPos)
    {
        float realRow = shipPos.z - Constants.BOTTOM_OFFSET;
        float realCol = shipPos.x - Constants.LEFT_OFFSET;
        int row = (int) Math.Floor(realRow / Constants.CELL_SIZE + 0.5);
        int col = (int) Math.Floor(realCol / Constants.CELL_SIZE + 0.5);
        return new Vector2(row, col);
    }

    public GameObject GetCellByPos(Vector2 pos)
    {
        return mapArr[(int)pos.x, (int)pos.y];
    }

    public void SetShipIntoArray(GameObject ship)
    {
        shipArr[ship.GetComponent<ShipController>().id] = ship;
    }

    public void RemoveShipFromArray(GameObject ship)
    {
        shipArr[ship.GetComponent<ShipController>().id] = null;
    }

    public void ResetCellStatus()
    {
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                mapArr[row, col].GetComponent<CellController>().ChangeStatus(CellStatus.NORMAL);
            }
        }
    }
}
