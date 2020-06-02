
using System.Collections.Generic;
class ShipsShooting
{
    public Ship GetShipFromPart(Player currentPlayer, Cell cell)
    {
        foreach (Ship ship in currentPlayer.shipsList)
        {
            if (ship.CheckIfAnIntactPart(cell))
            {
                return ship;
            }
        }
        return null;
    }

    public bool CheckIfWholeShipExploded(Ship currentShip)
    {
        foreach (KeyValuePair<Cell, PART_SHIP>  partShip in currentShip.arrPartShip)
        {
            if (partShip.Value == PART_SHIP.INTACT)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckIfAllShipsExploded(Player currentPlayer)
    {
        foreach (Ship ship in currentPlayer.shipsList)
        {
            if (!ship.isTakenDown)
            {
                return false;
            }
        }
        return true;
    }
}
