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
    }
    public void OnUpdate()
    {
        CheckDistanceToLeader();
        _boid.ApplySeparation();
    }
    public void CheckDistanceToLeader()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance > _boid._distanceToLeader)
            _fsm.ChangeState(FSM.State.Move);
    }
}
