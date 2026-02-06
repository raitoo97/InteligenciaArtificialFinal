using UnityEngine;
public class MoveBoidState : IState
{
    private Leader _leader;
    private Agent _agent;
    private Boid _boid;
    public MoveBoidState(Leader leader, Agent agent, Boid boid)
    {
        _leader = leader;
        _agent = agent;
        _boid = boid;
    }
    public void OnEnter()
    {
        _agent.ChangeMove(true);
    }
    public void OnUpdate()
    {
        FlockingAndArrive();
    }
    public void OnExit()
    {
        _agent.ChangeMove(false);
    }
    public void FlockingAndArrive()
    {
        _agent.ApplyArrive(_leader.transform.position);
        _boid.ApplySeparation();
    }
}
