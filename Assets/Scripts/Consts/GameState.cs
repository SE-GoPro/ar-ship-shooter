using System;
public class GameState
{
    public State CurrentState;

    public string HostPlayerId = "";
    public string GuestPlayerId = "";

    public string HostPlayerShips = "";
    public string GuestPlayerShips = "";

    public string CurrentTurn = "";

    public string Target = "";

    public int HostPlayerHealth = Constants.MAX_HP;
    public int GuestPlayerHealth = Constants.MAX_HP;

    public bool Hit = false;

    public string PlayerWin = "";

    public GameState Copy()
    {
        GameState cloned = (GameState) this.MemberwiseClone();
        // Clone deeper object if needed
        cloned.CurrentState = CurrentState;
        return cloned;
    }

    public T GetAttribute<T>(string fieldName)
    {
        return (T)typeof(GameState).GetField(fieldName).GetValue(this);
    }
}
