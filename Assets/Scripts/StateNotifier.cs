public interface StateNotifier
{
    void RegisterListener(StateListener listener);
    void NotifyNewState(State prevState, State currentState);
}
