using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class ShipsShootingExample
{
    static void Main(string[] args)
    {
        Cell declaredCell = new Cell(1, 2);
        Player currentPlayer = new Player();
        ShipsShooting ss = new ShipsShooting();
        Ship currentShip = ss.GetShipFromPart(currentPlayer, declaredCell);
        if (currentShip == null)
        {
            reRenderWaterSplashed(declaredCell);
        }
        else
        {
            reRenderPartShipExploded(declaredCell);
            if (ss.CheckIfWholeShipExploded(currentShip))
            {
                if (ss.CheckIfAllShipsExploded(currentPlayer))
                {
                    endGame();
                }
                else
                {
                    reRenderWholeShipExploded(currentShip);
                    currentShip.isTakenDown = true;
                }
            }
        }
    }
}
