using System;
using System.Collections;
class ShipsGenerator
{
    public int MAP_SIZE;
    public int[] lengthList;
    public int[,] mapArr;
    public ArrayList shipsList = new ArrayList();
    public int idShip = 1;

    /// <summary>
    /// A constructor to initialize MAP_SIZE, lengthList and mapArr
    /// </summary>
    /// <param name="MAP_SIZE"></param>
    /// <param name="lengthList"></param>
    public ShipsGenerator(int MAP_SIZE, int[] lengthList)
    {
        this.lengthList = lengthList;
        this.MAP_SIZE = MAP_SIZE;
        mapArr = new int[MAP_SIZE, MAP_SIZE];
        // Initialize map
        for (int i = 0; i < MAP_SIZE; i++)
        {
            for (int j = 0; j < MAP_SIZE; j++)
            {
                mapArr[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// Print the whole mapArr
    /// </summary>
    public void PrintMap()
    {
        for (int i = 0; i < MAP_SIZE; i++)
        {
            for (int j = 0; j < MAP_SIZE; j++)
            {
                Console.Write(mapArr[i, j] + " ");
            }
            Console.Write("\n");
        }
        Console.Write("\n\n");
    }

    /// <summary>
    /// Print the shipsList
    /// </summary>
    public void PrintShipsList()
    {
        foreach (Ship ship in shipsList)
        {
            Console.WriteLine("ID: " + ship.id);
            Console.WriteLine("Length: " + ship.length);
            Console.WriteLine("Dir: " + ship.dir);
            Console.WriteLine("Root: {");
            Console.WriteLine("  x: " + ship.root.x);
            Console.WriteLine("  y: " + ship.root.y);
            Console.WriteLine("}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Check if the coordination is available for the ship to be placed here?
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="deltaHorizontal"></param>
    /// <param name="deltaVertical"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CheckAvailable(int x, int y, int deltaHorizontal, int deltaVertical, int length)
    {
        // If that cell has a part of ship. RETURN FALSE
        if (mapArr[y, x] != 0) return false;
        // If there is a block towards the direction from that cell. RETURN FALSE
        for (int tempCol = x, tempRow = y, stepExpand = 0;
            stepExpand < length;
            tempCol += deltaHorizontal, tempRow += deltaVertical, stepExpand++)
        {
            if (tempCol > MAP_SIZE - 1 || tempCol < 0 || tempRow > MAP_SIZE - 1 || tempRow < 0
                || (mapArr[tempRow, tempCol] != 0))
            {
                return false;
            }
        }
        // If surrounding of that ship is adjacent to another ship. RETURN FALSE
        if (length == 1
            && !CheckSurrounding.CheckOneCell(MAP_SIZE, mapArr, x, y))
            return false;
        if (length > 1 && deltaHorizontal != 0
            && !CheckSurrounding.CheckHorizontal(MAP_SIZE, mapArr, x, y, deltaHorizontal, length))
            return false;
        if (length > 1 && deltaVertical != 0
            && !CheckSurrounding.CheckVertical(MAP_SIZE, mapArr, x, y, deltaVertical, length))
            return false;
        return true;
    }

    /// <summary>
    /// Generate 1 ship and add it to shipsList
    /// </summary>
    /// <param name="length"></param>
    public void GenerateOneShip(int length)
    {
        /** PICK (x, y) AND DIRECTION OF A CELL*/
        Random rand = new Random();
        // 1 = LEFT && 2 = UP && 3 = RIGHT && 4 = DOWN
        int dir = rand.Next(4) + 1;
        // Direction entity has 3 properties: dir, deltaHorizontal, deltaVertical
        Direction newDir = new Direction(dir);
        // Pick a random number for (x,y)
        int x = rand.Next(MAP_SIZE - 1) + 0;
        int y = rand.Next(MAP_SIZE - 1) + 0;
        // Check if those numbers are usable?
        while (true)
        {
            if (CheckAvailable(x, y, newDir.deltaHorizontal, newDir.deltaVertical, length))
            {
                break;
            }
            rand = new Random();
            dir = rand.Next(4) + 1;
            x = rand.Next(MAP_SIZE - 1) + 0;
            y = rand.Next(MAP_SIZE - 1) + 0;
        }

        /** ADD INFO OF THAT SHIP TO THE ARRAYLIST*/
        Cell root = new Cell(x, y);
        Ship newShip = new Ship(idShip, length, dir, root);
        shipsList.Add(newShip);

        /** ADD THAT SHIP TO MAP AND UPDATE MAP*/
        for (int step = 1; step <= length; step++)
        {
            mapArr[y, x] = length;
            y += newDir.deltaVertical;
            x += newDir.deltaHorizontal;
        }
    }

    /// <summary>
    /// Generate all ships in random coordinations
    /// </summary>
    public void GenerateAllRandomShips()
    {
        foreach (int length in lengthList)
        {
            GenerateOneShip(length);
            idShip++;
        }
    }
}
