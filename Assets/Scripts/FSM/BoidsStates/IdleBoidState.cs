using UnityEngine;
public class IdleBoidState : IState
{
    private FSM _fsm;
    private Leader _leader;
    private Agent _agent;
    private Boid _boid;
    public IdleBoidState(Boid boid,Leader leader,Agent agent,FSM fsm)
    {
        _leader = leader;
        _agent = agent;
        _fsm = fsm;
        _boid = boid;
    }
    public void OnEnter()
    {
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
    }
    public void OnUpdate()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance > _boid._distanceToLeader)
        {
            _fsm.ChangeState(FSM.State.Move);
            return;
        }
        bool hasNeighbors = false;
        foreach (var boid in _boid._neigboards)
        {
            if (boid == _boid) continue;
            float dist = Vector3.Distance(_boid.transform.position, boid.transform.position);
            if (dist < _boid.radiusSeparation)
            {
                hasNeighbors = true;
                break;
            }
        }
        float distToLeader = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        bool tooCloseToLeader = distToLeader < _boid.radiusSeparation;
        if (hasNeighbors || tooCloseToLeader)
        {
            _boid.ApplySeparation();
        }
        else
        {
            _agent.ChangeMove(false);
        }
    }
}
