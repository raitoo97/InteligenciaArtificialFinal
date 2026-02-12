using System.Collections.Generic;
using UnityEngine;
public class SearchEnemyLeaderState : IState
{
    private Leader _leader;
    private Agent _agent;
    private FSM _fsm;
    private Transform _target;
    private float _searchTimer;
    private float _searchInterval = 0.5f;
    private Vector3 _lastKnowPosition;
    private float _distanceTreshold = 5f;
    public SearchEnemyLeaderState(Leader leader, Agent agent, FSM fsm)
    {
        _leader = leader;
        _agent = agent;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        _target = ChooseRandomEnemy();
        _searchTimer = _searchInterval;
        _leader.ClearPath();
        _agent.ChangeMove(true);
        if (_target != null)
            _lastKnowPosition = _target.position;
    }
    public void OnExit()
    {
        _leader.ClearPath();
    }
    public void OnUpdate()
    {
        if (_leader.Life.GetLife <= _leader.MinLifeToRetreat)
        {
            _leader.ClearPath();
            _fsm.ChangeState(FSM.State.Retreat);
            return;
        }
        if (_target == null)
        {
            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                _searchTimer = _searchInterval;
                _target = ChooseRandomEnemy();
            }
            return;
        }
        var visibleEnemy = _leader.GetClosestVisibleEnemy();
        if (visibleEnemy != null)
        {
            _leader.ClearPath();
            _fsm.ChangeState(FSM.State.Attack);
            return;
        }
        if (_target != null)
        {
            float distance = Vector3.Distance(_target.position, _lastKnowPosition);
            if (_leader.MainPath.Count == 0 || distance > _distanceTreshold)
            {
                var isOnSight = LineOfSight.IsOnSight(_leader.transform.position, _target.position);
                if (isOnSight)
                    _leader.GoDirectToTarget(_target.position);
                else
                    _leader.CalculatePathToTarget(_target.position);
                _lastKnowPosition = _target.position;
            }
        }
        if (_leader.MainPath.Count > 0)
        {
            _leader.MoveAlongPath();
        }
    }
    private Transform ChooseRandomEnemy()
    {
        List<Boid> enemies = new List<Boid>();
        foreach (var boid in BoidManager.instance.GetBoids)
        {
            if (boid == null) continue;
            if (boid.typeBoid !=(_leader.IsVioletLeader ? TypeBoid.VioletTeam : TypeBoid.BlueTeam))
                enemies.Add(boid);
        }
        if (enemies.Count > 0)
        {
            int index = Random.Range(0, enemies.Count);
            return enemies[index].transform;
        }
        var enemyLeader = LeaderManager.instance.GetLeader(_leader);
        if (enemyLeader != null)
            return enemyLeader.transform;
        return null;
    }
}
