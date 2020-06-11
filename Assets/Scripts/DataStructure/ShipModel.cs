using System;
public class ShipModel
{
    public int rootRow;
    public int rootCol;
    public int dir;
    public int length;
    public int id;

    public ShipModel(int rootRow, int rootCol, int dir, int length, int id)
    {
        this.rootRow = rootRow;
        this.rootCol = rootCol;
        this.dir = dir;
        this.length = length;
        this.id = id;
    }
}
