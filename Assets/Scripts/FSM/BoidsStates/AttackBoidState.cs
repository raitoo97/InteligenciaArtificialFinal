using UnityEngine;
public class AttackBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private Transform _target;
    private float _attackRange = 10f;
    private float _minDistance = 3f;
    private float _maxCooldown;
    private float _currentCooldown;
    private bool _hasNeighbors;
    private bool _isStopped;
    public AttackBoidState(Boid boid,Agent agent)
    {
        _agent = agent;
        _boid = boid;
    }
    public void OnEnter()
    {
        _target = FindTarget();
        _maxCooldown = 3;
        _isStopped = false;
        Debug.Log("Boid enter attack");
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
        Debug.Log("EXIT Attack");
    }
    public void OnUpdate()
    {
        if (_target == null)
        {
            _target = FindTarget();
            if (_target == null)
                return;
        }
        Vector3 dir = _target.position - _boid.transform.position;
        var dist = dir.magnitude;
        if (dist < _minDistance)
        {
            _agent.ApplyFlee(_target.position);
            return;
        }
        if (dist > _attackRange)
        {
            bool hasLOS = LineOfSight.IsOnSight(_boid.transform.position,_target.position);
            if (hasLOS)
                _boid.GoDirectToTarget(_target.position);
            else
                _boid.CalculatePathToTarget(_target.position);
            return;
        }
        else
        {
            bool leaderTooClose = _boid.HasLeaderTooClose();
            _boid.CheckHasNeighbors(ref _hasNeighbors);
            bool shouldMove = _hasNeighbors || leaderTooClose;
            if (shouldMove && _isStopped)
            {
                _agent.ChangeMove(true);
                _isStopped = false;
            }
            else if (!shouldMove && !_isStopped)
            {
                _agent.ChangeMove(false);
                _isStopped = true;
            }
            if (shouldMove)
            {
                _boid.ApplySeparation();
            }
        }
        _boid.RotateTo(dir);
        TryShoot();
    }
    private Transform FindTarget()
    {
        float chance = Random.value;
        if(chance < 0.5f)
        {
            var enemyLeader = LeaderManager.instance.GetLeader(_boid.Leader);
            if (enemyLeader != null &&LineOfSight.IsOnSight(_boid.transform.position, enemyLeader.transform.position))
            {
                return enemyLeader.transform;
            }
        }
        var allBoids = BoidManager.instance.GetBoids;
        foreach (var boid in allBoids)
        {
            if (!LineOfSight.IsOnSight(_boid.transform.position, boid.transform.position)) continue;
            if (boid.typeBoid != _boid.typeBoid)
                return boid.transform;
        }
        return null;
    }
    private void TryShoot()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
            return;
        }
        var bullet = PoolBullet.instance.GetBullet();
        bullet.transform.position = _boid.transform.position;
        bullet.transform.rotation = _boid.transform.rotation;
        bullet.GetComponent<Bullet>().Shoot(_boid.typeBoid);
        _currentCooldown = _maxCooldown;
    }
}
