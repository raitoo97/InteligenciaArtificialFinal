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
        Debug.Log("Entering Idle State");
    }

    public void OnExit()
    {
        _agent.ChangeMove(true);
    }
    public void OnUpdate()
    {
        CheckDistanceToLeader();
        if (!HasCloseNeigboards())
        {
            _agent.ChangeMove(false);
        }
    }
    public void CheckDistanceToLeader()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance > _boid._distanceToLeader)
            _fsm.ChangeState(FSM.State.Move);
    }
    public bool HasCloseNeigboards()
    {
        foreach(var boid in _boid._neigboards)
        {
            if (boid == _boid) continue;
            var distance = Vector3.Distance(_boid.transform.position, boid.transform.position);
            if (distance < _boid.radiusSeparation)
                return true;
        }
        return false;
    }
}
