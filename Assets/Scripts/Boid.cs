using System.Collections.Generic;
using UnityEngine;
public enum TypeBoid
{
    BlueTeam,
    VioletTeam
}
public class Boid : Agent , IFlockingSeparation
{
    private FSM _fsm;
    [Header("BoidConfig")]
    [SerializeField]public List<Boid> _neigboards = new List<Boid>();
    private List<Vector3> _currentPath = new List<Vector3>();
    public TypeBoid typeBoid;
    public float weightSeparation;
    public float leaderSeparationWeight;
    [SerializeField]private Leader _leaderRef;
    [Range(0f, 5f)] public float radiusSeparation;
    [Range(0f, 4f)] public  float _distanceToLeader;
    [SerializeField] private float _maxLife;
    private Life _life;
    [Header("FOV")]
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    private bool _alreadyAlerted;
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.State.Move, new MoveBoidState(this,_leaderRef,_fsm));
        _fsm.AddState(FSM.State.Idle, new IdleBoidState(this,_leaderRef,this,_fsm));
        _fsm.AddState(FSM.State.Attack, new AttackBoidState(this,this,_fsm));
        _fsm.AddState(FSM.State.SearchEnemy, new SearchEnemyBoidState(this,this,_fsm));
        _life = new Life(_maxLife);
    }
    protected override void Start()
    {
        base.Start();
        _fsm.ChangeState(FSM.State.Idle);
    }
    protected override void Update()
    {
        _fsm?.OnUpdate();
        base.Update();
    }
    public Vector3 Separation(List<Boid> boids, float range)
    {
        var desired = Vector3.zero;
        foreach(var boid in boids)
        {
            var direction = this.transform.position - boid.transform.position;
            var distance = direction.magnitude;
            if (distance > range || boid == this) continue;
            desired += direction;
        }
        if (desired == Vector3.zero)
            return Vector3.zero;
        desired = desired.normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    public Vector3 SeparationFromLeader()
    {
        if (_leaderRef == null) return Vector3.zero;
        var direction = this.transform.position - _leaderRef.transform.position;
        var distance = direction.magnitude;
        if (distance > radiusSeparation) return Vector3.zero;
        var desired = direction.normalized * _maxSpeed;
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        return steering;
    }
    public void ApplySeparation()
    {
        AddForce(Separation(_neigboards, radiusSeparation) * weightSeparation);
        AddForce(SeparationFromLeader() * leaderSeparationWeight);
    }
    public void RotateTo(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    public bool DetectEnemy()
    {
        if (_alreadyAlerted) return true;
        var enemyLeader = LeaderManager.instance.GetLeader(_leaderRef);
        if (enemyLeader != null && FOV.InFieldOfView(enemyLeader.transform, this.transform, _viewRadius, _viewAngle))
        {
            AlertAllies();
            _alreadyAlerted = true;
            return true;
        }
        var allBoids = BoidManager.instance.GetBoids;
        List<Boid> enemyBoids;
        if (_leaderRef.IsVioletLeader)
        {
            enemyBoids = allBoids.FindAll(b => b.typeBoid == TypeBoid.BlueTeam);
        }
        else
        {
            enemyBoids = allBoids.FindAll(b => b.typeBoid == TypeBoid.VioletTeam);
        }
        foreach (var boid in enemyBoids)
        {
            var anyBoidInFOV = FOV.InFieldOfView(boid.transform, this.transform, _viewRadius, _viewAngle);
            if (anyBoidInFOV)
            {
                AlertAllies();
                _alreadyAlerted = true;
                return true;
            }
        }
        return false;
    }
    public void ResetAlert()
    {
        _alreadyAlerted = false;
    }
    public void ForceAttack()
    {
        _fsm.ChangeState(FSM.State.Attack);
    }
    public void AlertAllies()
    {
        var allBoids = BoidManager.instance.GetBoids;
        foreach (var boid in allBoids)
        {
            if (boid.typeBoid == this.typeBoid)
            {
                boid.ForceAttack();
            }
        }
        _leaderRef.ForceAttack();
    }
    public void CheckHasNeighbors(ref bool _hasNeighbors)
    {
        _hasNeighbors = false;
        foreach (var boid in _neigboards)
        {
            if (boid == this) continue;
            float dist = Vector3.Distance(this.transform.position, boid.transform.position);
            if (dist < radiusSeparation)
            {
                _hasNeighbors = true;
                return;
            }
        }
    }
    public bool HasLeaderTooClose()
    {
        if (_leaderRef == null) return false;
        float dist = Vector3.Distance(this.transform.position, _leaderRef.transform.position);
        return dist < radiusSeparation;
    }
    public void MoveAlongPath()
    {
        if (_currentPath.Count == 0) return;
        var target = _currentPath[0];
        var dir = target - this.transform.position;
        RotateTo(dir);
        ApplyArrive(target);
        if (Vector3.Distance(this.transform.position, target) < 1.5f)
            _currentPath.RemoveAt(0);
    }
    public void GoDirectToTarget(Vector3 target)
    {
        ClearPath();
        _currentPath.Add(target);
    }
    public void CalculatePathToTarget(Vector3 target)
    {
        ClearPath();
        var path = PathFinding.CalculateTheta(this.transform.position, target);
        _currentPath.AddRange(path);
    }
    public void ClearPath()
    {
        _currentPath.Clear();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, radiusSeparation);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, _distanceToLeader);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);
        Vector3 rightDir = Quaternion.Euler(0, _viewAngle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -_viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + rightDir * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + leftDir * _viewRadius);
    }
    private void OnDestroy()
    {
        _fsm.RemoveState(FSM.State.Idle);
        _fsm.RemoveState(FSM.State.Move);
        _fsm.RemoveState(FSM.State.Attack);
        _fsm.RemoveState(FSM.State.SearchEnemy);
        _fsm = null;
        _life = null;
    }
    public Leader Leader { get => _leaderRef; }
    public Life Life { get => _life; }
    public List<Vector3> GetPath { get => _currentPath; }
    public float ViewRadius { get => _viewRadius; }
}
