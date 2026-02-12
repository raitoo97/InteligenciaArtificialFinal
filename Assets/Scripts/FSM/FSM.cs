using System.Collections.Generic;
public class FSM
{
    public enum State
    {
        Idle,
        Move,
        Attack,
        SearchEnemy,
        Retreat
    }
    Dictionary<State,IState> _allStates = new Dictionary<State, IState>();
    private IState _currentState;
    private State _currentStateKey;
    public void AddState(State key,IState value)
    {
        if (_allStates.ContainsKey(key)) return;
        _allStates.Add(key, value);
    }
    public void RemoveState(State key) 
    {
        if (!_allStates.ContainsKey(key)) return;
        _allStates.Remove(key);
    }
    public void ChangeState(State key)
    {
        if (!_allStates.ContainsKey(key)) return;
        _currentState?.OnExit();
        _currentState = _allStates[key];
        _currentStateKey = key;
        _currentState?.OnEnter();
    }
    public void OnUpdate()
    {
        _currentState?.OnUpdate();
    }
    public State CurrentState { get => _currentStateKey; }
}
