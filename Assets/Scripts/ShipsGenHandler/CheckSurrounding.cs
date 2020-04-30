class CheckSurrounding
{
    /// <summary>
    /// If deltaVertical != 0 it means the ship is expanding towards VERTICAL. This function is used to check if
    /// that place is available
    /// </summary>
    /// <param name="MAP_SIZE"></param>
    /// <param name="mapArr"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="deltaVertical"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static bool CheckVertical(int MAP_SIZE, int[,] mapArr, int x, int y, int deltaVertical, int length)
    {
        // First we get the 4 corner of rectangle
        int startCol = x - 1, endCol = x + 1;
        int startRow = y - deltaVertical, endRow = y + length * deltaVertical;
        // We have the center parts of rectangle is the SHIP
        // We have the border parts of rectangle is SURROUNDING
        // So we will check the SURROUNGING if they are available?

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

    /// <summary>
    /// If deltaHorizontal != 0 it means the ship is expanding towards HORIZONTAL. This function is used to check if
    /// that place is available
    /// </summary>
    /// <param name="MAP_SIZE"></param>
    /// <param name="mapArr"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="deltaHorizontal"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static bool CheckHorizontal(int MAP_SIZE, int[,] mapArr, int x, int y, int deltaHorizontal, int length)
    {
        // First we get the 4 corner of rectangle
        int startCol = x - deltaHorizontal, endCol = x + length * deltaHorizontal;
        int startRow = y - 1, endRow = y + 1;
        // We have the center parts of rectangle is the SHIP
        // We have the border parts of rectangle is SURROUNDING
        // So we will check the SURROUNGING if they are available?

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

    /// <summary>
    /// If length = 1 then we don't have to see if this ship is moving towards vertical or horizontal. Just check the
    /// square around that ship.
    /// </summary>
    /// <param name="MAP_SIZE"></param>
    /// <param name="mapArr"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool CheckOneCell(int MAP_SIZE, int[,] mapArr, int x, int y)
    {
        // First we get the 4 corner of square
        int startCol = x - 1, endCol = x + 1;
        int startRow = y - 1, endRow = y + 1;
        // We have the center cell of square is the SHIP
        // We have the border parts of square is SURROUNDING
        // So we will check the SURROUNGING if they are available?
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
}
