class Direction
{
    public int dir = 0;
    public int deltaHorizontal = 0;
    public int deltaVertical = 0;

    public Direction(int dir)
    {
        this.dir = dir;
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
    }
}
