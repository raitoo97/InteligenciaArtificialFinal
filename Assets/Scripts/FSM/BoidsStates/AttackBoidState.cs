using UnityEngine;
public class AttackBoidState : IState
{
    private Agent _agent;
    public AttackBoidState(Agent agent)
    {
        _agent = agent;
    }
    public void OnEnter()
    {
        _agent.ChangeMove(false);
        Debug.Log("Enter Attack Boid");
    }
    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
    }
}
