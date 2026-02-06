using UnityEngine;
public class IdleBoidState : IState
{
    private FSM _fsm;
    private Leader _leader;
    private Agent _agent;
    private Boid _boid;
    private bool _hasNeighbors;
    private bool _isStopped;
    public IdleBoidState(Boid boid,Leader leader,Agent agent,FSM fsm)
    {
        _leader = leader;
        _agent = agent;
        _fsm = fsm;
        _boid = boid;
    }
    public void OnEnter()
    {
        _isStopped = false;
        CheckHasNeighbors();
    }

    public void OnUpdate()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance > _boid._distanceToLeader)
        {
            _fsm.ChangeState(FSM.State.Move);
            return;
        }
        CheckHasNeighbors();
        if (_hasNeighbors && _isStopped)
        {
            _agent.ChangeMove(true);
            _isStopped = false;
        }
        else if (!_hasNeighbors && !_isStopped)
        {
            _agent.ChangeMove(false);
            _isStopped = true;
        }
        if (_hasNeighbors)
        {
            _boid.ApplySeparation();
        }
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
    }
    private void CheckHasNeighbors()
    {
        _hasNeighbors = false;
        foreach (var boid in _boid._neigboards)
        {
            if (boid == _boid) continue;
            float dist = Vector3.Distance(_boid.transform.position, boid.transform.position);
            if (dist < _boid.radiusSeparation)
            {
                _hasNeighbors = true;
                return;
            }
        }
    }
}
