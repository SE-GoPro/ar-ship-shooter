using System.Collections.Generic;
using System.Linq;

enum PART_SHIP
{
    DESTROYED,
    INTACT,
} 

class Ship
{
    public int id;
    public int length;
    public int dir;
    public Cell root;
    public bool isTakenDown;
    public Dictionary<Cell, PART_SHIP> arrPartShip = new Dictionary<Cell, PART_SHIP>();

    public Ship(int id, int length, int dir, Cell root)
    {
        this.id = id;
        this.length = length;
        this.dir = dir;
        this.root = root;
    }

    public bool CheckIfAnIntactPart(Cell cell)
    {
        var currentStatusCell = (from part in this.arrPartShip
                    where part.Key == cell
                    select part.Value).FirstOrDefault();
        if (currentStatusCell == PART_SHIP.INTACT)
        {
            return true;
        }
        return false;
    }
}
