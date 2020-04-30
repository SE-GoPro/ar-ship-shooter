class Ship
{
    public int id;
    public int length;
    public int dir;
    public Cell root;

    public Ship(int id, int length, int dir, Cell root)
    {
        this.id = id;
        this.length = length;
        this.dir = dir;
        this.root = root;
    }
}
