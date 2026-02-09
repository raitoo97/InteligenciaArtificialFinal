using UnityEngine;
public class AttackBoidState : IState
{
    private Boid _boid;
    private Agent _agent;
    private Transform _target;
    private float _attackRange = 10f;
    private float _fireRate = 1f;
    private float _nextFireTime;
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
        _nextFireTime = Time.time;
        _isStopped = false;
    }
    public void OnExit()
    {
        _agent.ChangeMove(true);
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
        _boid.RotateTo(dir);
        if (dist > _attackRange)
        {
            _agent.ApplySeek(_target.position);
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
        if (Time.time < _nextFireTime)
            return;
        _nextFireTime = Time.time + _fireRate;
        Debug.Log("Boid dispara");
    }
}
