using System;
public class TargetModel
{
    public int Row;
    public int Col;
    public string TargetId;

    public TargetModel(int Row, int Col, string TargetId)
    {
        this.Row = Row;
        this.Col = Col;
        this.TargetId = TargetId;
    }
}
