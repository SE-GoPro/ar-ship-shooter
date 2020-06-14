using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class FieldMapController : MonoBehaviour
{
    public int size = Constants.MAP_SIZE;
    public float LeftOffset = Constants.LEFT_OFFSET;
    public float BottomOffset = Constants.BOTTOM_OFFSET;
    public GameObject CellPrefab = null;
    public GameObject[,] mapArr;
    public GameObject[] shipArr;
    public GameObject[] allShips;

    public GameObject SceneController;

    public GameObject SelectedCell = null;
    public bool IsMyField = false;
    public bool Selectable = false;

    public CellController LastHitCellCon = null;
    public bool IsLastHitDestroyShip = false;

    public void Awake()
    {
        // Init ships
        allShips = new GameObject[Constants.SHIP_AMOUNT];
    }

    public void Init()
    {
        // Init map
        mapArr = new GameObject[Constants.MAP_SIZE, Constants.MAP_SIZE];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                GameObject cell = Instantiate(
                    CellPrefab,
                    Vector3.zero,
                    CellPrefab.transform.rotation,
                    gameObject.transform
                );
                cell.GetComponent<Transform>().transform.localPosition = new Vector3(
                    col * Constants.CELL_SIZE + LeftOffset,
                    Constants.CELL_ELEVATION,
                    row * Constants.CELL_SIZE + BottomOffset
                );
                cell.GetComponent<CellController>().row = row;
                cell.GetComponent<CellController>().col = col;
                cell.GetComponent<CellController>().fieldMap = this.gameObject;
                mapArr[row, col] = cell;
            }
        }
        shipArr = new GameObject[Constants.SHIP_AMOUNT];
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
                numMapArr[row, col] = Constants.EMPTY_CELL_ID;
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

    public bool CheckValidCellCord(int row, int col)
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
                if (numMapArr[(int)cell.x, (int)cell.y] != Constants.EMPTY_CELL_ID)
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
        float realRow = shipPos.z - BottomOffset;
        float realCol = shipPos.x - LeftOffset;
        int row = (int) Math.Floor(realRow / Constants.CELL_SIZE + 0.5);
        int col = (int) Math.Floor(realCol / Constants.CELL_SIZE + 0.5);
        return new Vector2(row, col);
    }

    public GameObject GetCellByPos(Vector2 pos)
    {
        return mapArr[(int)pos.x, (int)pos.y];
    }

    public GameObject GetCellByPos(int row, int col)
    {
        return mapArr[row, col];
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

    public void ResetAllShips()
    {
        foreach (GameObject ship in shipArr)
        {
            if (ship != null)
            {
                ship.GetComponent<ShipController>().ResetShip();
            }
        }
    }

    public void AutoArrangeShips()
    {
        // reset all arranged ships
        ResetAllShips();
        foreach (GameObject ship in allShips)
        {
            ShipController shipCon = ship.GetComponent<ShipController>();
            System.Random rand = new System.Random();

            int ranDir = 1;
            int randomRow = 0;
            int randomCol = 0;
            bool isValid = false;
            while (!isValid)
            {
                ranDir = rand.Next(4) + 1;
                randomRow = rand.Next(Constants.MAP_SIZE);
                randomCol = rand.Next(Constants.MAP_SIZE);
                shipCon.SetDirection(ranDir);
                GameObject cell = mapArr[randomCol, randomRow];
                isValid = CheckValidShipPos(ship, new Vector2(randomRow, randomCol));
            }
            shipCon.SetShipToMap(randomRow, randomCol, ranDir);
        }
        ResetCellStatus();
        SoundManager.Instance.PlaySound(SoundManager.Sound.BOAT_TO_WATER);
    }

    public string GetSerializedShips()
    {
        string serializedShips = "[";
        for (int i = 0; i < Constants.SHIP_AMOUNT; i++)
        {
            if (shipArr[i] == null)
            {
                serializedShips += "null";
            } else
            {
                serializedShips += shipArr[i].GetComponent<ShipController>().Serialize();
            }
            serializedShips += "|";
        }
        serializedShips = serializedShips.Trim('|');
        serializedShips += "]";
        return serializedShips;
    }

    public ShipModel[] DeserializeShips(string serializedShips)
    {
        serializedShips = serializedShips.Trim('[');
        serializedShips = serializedShips.Trim(']');
        string[] serializedShip = serializedShips.Split('|');
        ShipModel[] shipsModels = serializedShip.Select(item => JsonUtility.FromJson<ShipModel>(item)).ToArray();
        return shipsModels;
    }

    public void SelectCell(GameObject cell)
    {
        UnselectSelectedCell();
        SelectedCell = cell;
        SelectedCell.GetComponent<CellController>().ChangeStatus(CellStatus.SELECTED);
        SceneController.GetComponent<InGameController>().HUDManager.GetComponent<HUDController>().HUDAttackButton.SetActive(true);
    }

    public void UnselectSelectedCell()
    {
        if (SelectedCell != null)
        {
            SelectedCell.GetComponent<CellController>().ChangeStatus(CellStatus.NORMAL);
            SelectedCell = null;
            SceneController.GetComponent<InGameController>().HUDManager.GetComponent<HUDController>().HUDAttackButton.SetActive(false);
        }
    }

    public void CheckDestroyCell(GameObject cell)
    {
        Logger.Log("FieldMap: CheckDestroyCell");
        CellController cellCon = cell.GetComponent<CellController>();
        cellCon.Fired = true;
        int[,] numMapArr = GetNumericMap();
        int ShipIdAtCell = numMapArr[cellCon.row, cellCon.col];
        if (ShipIdAtCell == Constants.EMPTY_CELL_ID)
        {
            // If empty cell
            SoundManager.Instance.PlaySound(SoundManager.Sound.BOOM_MISS_WATER);
            StartCoroutine(DelayDecreaseHealth(0, 1));
        } else
        {
            // If cell has ship
            SoundManager.Instance.PlaySound(SoundManager.Sound.BOOM_HIT_WATER);
            LastHitCellCon = cellCon;
            IsLastHitDestroyShip = false;
            cellCon.HasShip = true;
            GameObject ship = GetComponent<FieldMapController>().shipArr[ShipIdAtCell];
            ship.GetComponent<ShipController>().Attacked++;
            // If the whole ship destroyed
            if (ship.GetComponent<ShipController>().Attacked == ship.GetComponent<ShipController>().length)
            {
                IsLastHitDestroyShip = true;
                ship.SetActive(true);
                // Show explosion
                Vector2[] cellsPos = GetCellsInShip(ship.GetComponent<ShipController>());
                foreach (Vector2 cellPos in cellsPos)
                {
                    GameObject currentCell = mapArr[(int)cellPos.x, (int)cellPos.y];
                    currentCell.GetComponent<CellController>().Boom.GetComponent<ParticleSystem>().Play();
                }
            }
            StartCoroutine(DelayDecreaseHealth(-1, 1));
        }
        cellCon.ChangeStatus(CellStatus.FIRED);
    }

    IEnumerator DelayDecreaseHealth(int offset, float wait)
    {
        yield return new WaitForSeconds(wait);
        GameManager.Instance.DecreaseHealth(offset, IsMyField);
    }
}
