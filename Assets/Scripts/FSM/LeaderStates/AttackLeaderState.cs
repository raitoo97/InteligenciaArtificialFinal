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
        Debug.Log("Attack State Leader");
        _agent.ChangeMove(false);
    }

    public void OnExit()
    {
        Debug.Log("Exit Attack State Leader");
         _agent.ChangeMove(true);
    }

    public void OnUpdate()
    {
    }
}
