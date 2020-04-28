using System;

class RandomShip
{
    public static int MAP_SIZE = 9;
    //public static int[] lengthList = { 2, 3, 3, 4, 4, 4 };
    public static int[] lengthList = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
    public static int[,] mapArr = new int[MAP_SIZE, MAP_SIZE];

    public static void InitializeMap()
    {
        for (int i = 0; i < MAP_SIZE; i++)
        {
            for (int j = 0; j < MAP_SIZE; j++)
            {
                mapArr[i, j] = 0;
            }
        }
    }

    public static void PrintMap(int[,] mapArr)
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

    public static bool CheckSurroundingHorizontal(int x, int y, int deltaHorizontal, int length)
    {
        int startCol = x - deltaHorizontal, endCol = x + length * deltaHorizontal;
        int startRow = y - 1, endRow = y + 1;
        // If direction is RIGHT then stepCol must be lower than endCol
        if (deltaHorizontal == 1)
        {
            for (int stepRow = startRow; stepRow <= endRow; stepRow++)
            {
                for (int stepCol = startCol; stepCol <= endCol; stepCol++)
                {
                    if (stepRow < 0 || stepRow > MAP_SIZE - 1) continue;
                    if (stepCol < 0 || stepCol > MAP_SIZE - 1) continue;
                    if (stepRow == y && stepCol != startCol && stepCol != endCol) continue;
                    if (mapArr[stepRow, stepCol] != 0) return false;
                }
            }
        }
        // If direction is LEFT then stepCol must be higher than endCol
        else
        {
            for (int stepRow = startRow; stepRow <= endRow; stepRow++)
            {
                for (int stepCol = startCol; stepCol >= endCol; stepCol--)
                {
                    if (stepRow < 0 || stepRow > MAP_SIZE - 1) continue;
                    if (stepCol < 0 || stepCol > MAP_SIZE - 1) continue;
                    if (stepRow == y && stepCol != startCol && stepCol != endCol) continue;
                    if (mapArr[stepRow, stepCol] != 0) return false;
                }
            }
        }
        return true;
    }

    public static bool CheckSurroundingVertical(int x, int y, int deltaVertical, int length)
    {
        int startCol = x - 1, endCol = x + 1;
        int startRow = y - deltaVertical, endRow = y + length * deltaVertical;
        // If direction is DOWN then stepRow must be lower than endRow
        if (deltaVertical == 1)
        {
            for (int stepRow = startRow; stepRow <= endRow; stepRow++)
            {
                for (int stepCol = startCol; stepCol <= endCol; stepCol++)
                {
                    if (stepRow < 0 || stepRow > MAP_SIZE - 1) continue;
                    if (stepCol < 0 || stepCol > MAP_SIZE - 1) continue;
                    if (stepCol == x && stepRow != startRow && stepRow != endRow) continue;
                    if (mapArr[stepRow, stepCol] != 0) return false;
                }
            }
        }
        // Else if direction is UP then stepRow must be higher than endRow
        else
        {
            for (int stepRow = startRow; stepRow >= endRow; stepRow--)
            {
                for (int stepCol = startCol; stepCol <= endCol; stepCol++)
                {
                    if (stepRow < 0 || stepRow > MAP_SIZE - 1) continue;
                    if (stepCol < 0 || stepCol > MAP_SIZE - 1) continue;
                    if (stepCol == x && stepRow != startRow && stepRow != endRow) continue;
                    if (mapArr[stepRow, stepCol] != 0) return false;
                }
            }
        }
        return true;
    }

    public static bool CheckSurrounding1Cell(int x, int y)
    {
        int startCol = x - 1, endCol = x + 1;
        int startRow = y - 1, endRow = y + 1;

        for (int stepRow = startRow; stepRow <= endRow; stepRow++)
        {
            for (int stepCol = startCol; stepCol <= endCol; stepCol++)
            {
                if (stepCol < 0 || stepCol > MAP_SIZE - 1) continue;
                if (stepRow < 0 || stepRow > MAP_SIZE - 1) continue;
                if (stepCol == x && stepRow == y) continue;
                if (mapArr[stepRow, stepCol] != 0) return false;
            }
        }
        return true;
    }

    public static bool CheckAvailable(int x, int y, int deltaHorizontal, int deltaVertical, int length)
    {
        // If that cell has a value other than 0, RETURN FALSE
        if (mapArr[y, x] != 0) return false;
        // If there is a block towards the direction from that cell. RETURN FALSE
        for (int tempCol = x, tempRow = y, stepExpand = 0;
            stepExpand < length;
            tempCol += deltaHorizontal, tempRow += deltaVertical, stepExpand++)
        {
            if (tempCol > MAP_SIZE - 1 || tempCol < 0 || tempRow > MAP_SIZE - 1 || tempRow < 0
                || (mapArr[tempRow, tempCol] != 0 && stepExpand < length))
            {
                return false;
            }
        }
        // If surrounding of that ship is adjacent to another ship. RETURN FALSE
        if (length > 1 && deltaHorizontal != 0 && !CheckSurroundingHorizontal(x, y, deltaHorizontal, length)) return false;
        if (length > 1 && deltaVertical != 0 && !CheckSurroundingVertical(x, y, deltaVertical, length)) return false;
        if (length == 1 && !CheckSurrounding1Cell(x, y)) return false;
        return true;
    }

    public static void GenerateOneShip(int length)
    {
        /** PICK (x, y) AND DIRECTION OF A CELL
         */
        Random rand = new Random();
        // 1 = LEFT && 2 = UP && 3 = RIGHT && 4 = DOWN
        int dir = rand.Next(4) + 1;
        // Pick a random number for (x,y)
        int x = rand.Next(MAP_SIZE - 1) + 0;
        int y = rand.Next(MAP_SIZE - 1) + 0;
        // Initialize deltaHorizontal, deltaVertical and stepLoop
        int deltaHorizontal, deltaVertical;
        int stepLoop = 0;
        while (true)
        {
            if (dir == 1)               // LEFT
            {
                deltaHorizontal = -1;
                deltaVertical = 0;
            }
            else if (dir == 2)          // UP
            {
                deltaHorizontal = 0;
                deltaVertical = -1;
            }
            else if (dir == 3)          // RIGHT
            {
                deltaHorizontal = 1;
                deltaVertical = 0;
            }
            else                        // DOWN
            {
                deltaHorizontal = 0;
                deltaVertical = 1;
            }

            if (CheckAvailable(x, y, deltaHorizontal, deltaVertical, length))
            {
                break;
            }
            rand = new Random();
            dir = rand.Next(4) + 1;
            x = rand.Next(MAP_SIZE - 1) + 0;
            y = rand.Next(MAP_SIZE - 1) + 0;
            stepLoop++;
        }

        /** DRAW A SHIP FROM THAT CELL
         */
        for (int step = 1; step <= length; step++)
        {
            mapArr[y, x] = length;
            y += deltaVertical;
            x += deltaHorizontal;
        }
    }

    public static void GenerateAllShips()
    {
        foreach (int length in lengthList)
        {
            GenerateOneShip(length);
        }
    }

    static void Main(string[] args)
    {
        InitializeMap();
        GenerateAllShips();
        PrintMap(mapArr);
    }
}

