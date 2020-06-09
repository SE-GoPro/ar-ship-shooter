using System;
public class GameState
{
    public State CurrentState;

    public string HostPlayerId = "";
    public string GuestPlayerId = "";

    public string HostPlayerShips = "";
    public string GuestPlayerShips = "";

    public string CurrentTurn = "";

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
