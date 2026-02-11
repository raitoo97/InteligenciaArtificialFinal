using System.Collections.Generic;
using UnityEngine;
public class MoveLeaderState : IState
{
    private List<Vector3> _mainPath = new List<Vector3>();
    private FSM _fsm;
    private Leader _leader;
    public MoveLeaderState(Leader leader,List<Vector3> mainPath ,FSM fsm)
    {
        _mainPath = mainPath;
        _leader = leader;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }
    public void OnUpdate()
    {
        if (_leader.Life.GetLife <= _leader.MinLifeToRetreat)
        {
            _leader.ClearPath();
            _fsm.ChangeState(FSM.State.Retreat);
            return;
        }
        if (_leader.DetectEnemy())
        {
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        if (_mainPath.Count <= 0)
        {
            _fsm.ChangeState(FSM.State.Idle);
            return;
        }
        _leader.MoveAlongPath();
    }
    public void OnExit()
    {
    }
}
