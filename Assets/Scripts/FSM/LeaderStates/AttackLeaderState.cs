using System.Collections.Generic;
using UnityEngine;
public class AttackLeaderState : IState
{
    private Agent _agent;
    private Leader _leader;
    private Transform _target;
    private Transform _gunSight;
    private float _maxCooldown;
    private float _currentCooldown;
    private bool _isStopped;
    public AttackLeaderState(Leader leader, Agent agent,Transform gunSight)
    {
        _leader = leader;
        _gunSight = gunSight;
        _agent = agent;
    }
    public void OnEnter()
    {
        _target = FindTarget();
        _maxCooldown = 3;
        _currentCooldown = Time.time;
        _isStopped = false;
        Debug.Log("Leader enter attack");
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
        Debug.Log("Leader exit attack");
    }
    public void OnUpdate()
    {
        if (_target == null)
        {
            _target = FindTarget();
            if (_target == null)
            {
                if (_isStopped)
                {
                    _agent.ChangeMove(true);
                    _isStopped = false;
                }
                Debug.Log("Leader No target found");
                return;
            }
        }
        Vector3 dir = _target.position - _leader.transform.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            _leader.RotateTo(dir);
        }
        bool hasLOS = LineOfSight.IsOnSight(_leader.transform.position, _target.position);
        if (hasLOS)
        {
            if (!_isStopped)
            {
                _agent.ChangeMove(false);
                _isStopped = true;
            }
            TryShoot();
        }
        else
        {
            if (_isStopped)
            {
                _agent.ChangeMove(true);
                _isStopped = false;
            }
        }
    }
    private Transform FindTarget()
    {
        float chance = Random.value;
        if (Random.value < 0.5f)
        {
            var enemyLeader = LeaderManager.instance.GetLeader(_leader);
            if (enemyLeader != null && LineOfSight.IsOnSight(_leader.transform.position, enemyLeader.transform.position))
                return enemyLeader.transform;
        }
        var allBoids = BoidManager.instance.GetBoids;
        List<Boid> visibleEnemies = new List<Boid>();
        foreach (var boid in allBoids)
        {
            if (boid == null) continue;
            if (!LineOfSight.IsOnSight(_leader.transform.position, boid.transform.position)) continue;
            bool isEnemy = (_leader.IsVioletLeader && boid.typeBoid == TypeBoid.BlueTeam) || (!_leader.IsVioletLeader && boid.typeBoid == TypeBoid.VioletTeam);
            if (!isEnemy) continue;
            visibleEnemies.Add(boid);
        }
        if (visibleEnemies.Count > 0)
        {
            int index = Random.Range(0, visibleEnemies.Count);
            return visibleEnemies[index].transform;
        }
        return null;
    }
    private void TryShoot()
    {
        if (_gunSight == null) return;
        if (Time.time - _currentCooldown < _maxCooldown)
            return;
        var bullet = PoolBullet.instance.GetBullet();
        bullet.transform.position = _gunSight.position;
        bullet.transform.rotation = _gunSight.rotation;
        bullet.GetComponent<Bullet>().Shoot(_leader.IsVioletLeader ? TypeBoid.VioletTeam : TypeBoid.BlueTeam);
        _currentCooldown = Time.time;
    }
}
