class ShipsGeneratorExample
{
    static void Main(string[] args)
    {
        int MAP_SIZE = 10;
        int[] lengthList = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

        ShipsGenerator sg = new ShipsGenerator(MAP_SIZE, lengthList);
        sg.GenerateAllRandomShips();
        sg.PrintShipsList();
        sg.PrintMap();
    }
}
