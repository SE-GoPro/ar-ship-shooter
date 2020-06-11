using System;
using System.Collections;
using UnityEngine;

public class ShipArranger
{
    public GameObject[] ships;

    public ShipArranger(GameObject[] ships)
    {
        this.ships = ships;
    }

    public void ArrangeShips()
    {
        int[,] numMapArr = new int[Constants.MAP_SIZE, Constants.MAP_SIZE];
        for (int row = 0; row < Constants.MAP_SIZE; row++)
        {
            for (int col = 0; col < Constants.MAP_SIZE; col++)
            {
                numMapArr[row, col] = Constants.EMPTY_CELL_ID;
            }
        }
    }
}
