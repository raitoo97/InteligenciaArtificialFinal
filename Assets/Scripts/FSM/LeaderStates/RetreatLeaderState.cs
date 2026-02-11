using UnityEngine;
public class RetreatLeaderState : IState
{
    private Leader _leader;
    private FSM _fsm;
    private Agent _agent;
    private Transform _safeSpot;
    public RetreatLeaderState(Leader leader, Agent agent, FSM fsm)
    {
        _leader = leader;
        _fsm = fsm;
        _agent = agent;
    }
    public void OnEnter()
    {
        _leader.ClearPath();
        _agent.ChangeMove(true);
        _safeSpot = GetSafeSpot();
        if (_safeSpot != null)
        {
            if (LineOfSight.IsOnSight(_leader.transform.position, _safeSpot.position))
                _leader.GoDirectToTarget(_safeSpot.position);
            else
                _leader.CalculatePathToTarget(_safeSpot.position);
        }
    }
    public void OnExit()
    {
        _leader.ClearPath();
    }
    public void OnUpdate()
    {
        if (_leader.MainPath.Count > 0)
        {
            _leader.MoveAlongPath();
            return;
        }
        _fsm.ChangeState(FSM.State.SearchEnemy);
    }
    private Transform GetSafeSpot()
    {
        _safeSpot = _leader.IsVioletLeader ? _leader._violetSecurePlace : _leader._blueSecurePlace;
        return _safeSpot;
    }
}
