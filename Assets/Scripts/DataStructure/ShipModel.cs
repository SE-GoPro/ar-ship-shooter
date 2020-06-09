using System;
public class ShipModel
{
    public string rootRow;
    public string rootCol;
    public string dir;
    public string length;
    public string id;

    public ShipModel(string rootRow, string rootCol, string dir, string length, string id)
    {
        this.rootRow = rootRow;
        this.rootCol = rootCol;
        this.dir = dir;
        this.length = length;
        this.id = id;
    }
}
