public interface StateNotifier
{
    public void RegisterListener(StateListener listener);
    public void NotifyNewState(State prevState, State currentState);
}
