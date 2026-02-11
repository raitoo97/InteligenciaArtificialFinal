using System.Collections.Generic;
using UnityEngine;
public class IdleLeaderState : IState
{
    private Agent _agent;
    private List<Vector3> _mainPath = new List<Vector3>();
    private FSM _fsm;
    private Leader _leader;
    public IdleLeaderState(Leader leader, List<Vector3> mainPath,Agent agent, FSM fsm)
    {
        _mainPath = mainPath;
        _agent = agent;
        _fsm = fsm;
        _leader = leader;
    }
    public void OnEnter()
    {
        _agent.ChangeMove(false);
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
    }
    public void OnUpdate()
    {
        if (_leader.DetectEnemy())
        {
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        if (_mainPath.Count > 0)
        {
            _fsm.ChangeState(FSM.State.Move);
            return;
        }
    }
}
