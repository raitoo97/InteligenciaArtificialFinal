using UnityEngine;
public class MoveBoidState : IState
{
    private Leader _leader;
    private Agent _agent;
    private Boid _boid;
    private FSM _fsm;
    public MoveBoidState(Boid boid, Leader leader,Agent agent,FSM fsm)
    {
        _leader = leader;
        _agent = agent;
        _boid = boid;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }
    public void OnUpdate()
    {
        var distance = Vector3.Distance(_boid.transform.position, _leader.transform.position);
        if (distance < _boid._distanceToLeader)
        {
            _fsm.ChangeState(FSM.State.Idle);
            return;
        }
        _boid.ApplySeparation();
        _agent.ApplyArrive(_leader.transform.position);
    }
    public void OnExit()
    {
    }
}
