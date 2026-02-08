using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("Attack State");
        _agent.ChangeMove(false);
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
