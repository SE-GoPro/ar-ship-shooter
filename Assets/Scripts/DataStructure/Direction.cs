using UnityEngine;

class Direction
{
    public int dir = 0;
    public int deltaHorizontal = 0;
    public int deltaVertical = 0;
    public Quaternion rotation;

    public Direction(int dir)
    {
        this.dir = dir;
        if (dir == 1)               // LEFT
        {
            deltaHorizontal = -1;
            deltaVertical = 0;
            rotation = Quaternion.Euler(-90, -270, 0);
        }
        else if (dir == 2)          // UP
        {
            deltaHorizontal = 0;
            deltaVertical = 1;
            rotation = Quaternion.Euler(-90, -180, 0);
        }
        else if (dir == 3)          // RIGHT
        {
            deltaHorizontal = 1;
            deltaVertical = 0;
            rotation = Quaternion.Euler(-90, -90, 0);
        }
        else                        // DOWN
        {
            deltaHorizontal = 0;
            deltaVertical = -1;
            rotation = Quaternion.Euler(-90, 0, 0);
        }
    }
}