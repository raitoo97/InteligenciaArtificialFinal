using System.Collections.Generic;
using UnityEngine;
public class IdleLeaderState : IState
{
    private Agent _agent;
    private List<Vector3> _mainPath = new List<Vector3>();
    private FSM _fsm;
    public IdleLeaderState(List<Vector3> mainPath,Agent agent, FSM fsm)
    {
        _mainPath = mainPath;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _agent.StopMove();
    }
    public void OnExit()
    {
    }
    public void OnUpdate()
    {
        if(_mainPath.Count > 0)
            _fsm.ChangeState(FSM.State.Move);
    }
}
