using UnityEngine;
public class AttackLeaderState : IState
{
    private Agent _agent;
    public AttackLeaderState(Agent agent)
    {
        _agent = agent;
    }
    public void OnEnter()
    {
        _agent.ChangeMove(false);
        Debug.Log("Leader enter attack");
    }
    public void OnExit()
    {
         _agent.ChangeMove(true);
    }

    public void OnUpdate()
    {
    }
}
