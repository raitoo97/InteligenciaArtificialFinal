using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum TypeBoid
{
    BlueTeam,
    VioletTeam
}
public class Boid : Agent , IFlockingSeparation
{
    private FSM _fsm;
    [Header("BoidConfig")]
    public List<Boid> _neigboards = new List<Boid>();
    private List<Vector3> _currentPath = new List<Vector3>();
    public TypeBoid typeBoid;
    [SerializeField]private Transform _gunSight;
    [SerializeField]private Slider _slider;
    public float weightSeparation;
    public float leaderSeparationWeight;
    public float enemySeparationWeight;
    [SerializeField]private Leader _leaderRef;
    [Range(0f, 5f)] public float radiusSeparation;
    [Range(0f, 4f)] public  float _distanceToLeader;
    [SerializeField] private float _maxLife;
    private Life _life;
    public Transform _blueSecurePlace;
    public Transform _violetSecurePlace;
    [SerializeField]private float _minLifeToRetreat;
    [Header("FOV")]
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    private bool _alreadyAlerted;
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.State.Move, new MoveBoidState(this,_leaderRef,_fsm));
        _fsm.AddState(FSM.State.Idle, new IdleBoidState(this,_leaderRef,this,_fsm));
        _fsm.AddState(FSM.State.Attack, new AttackBoidState(_gunSight, this,this,_fsm));
        _fsm.AddState(FSM.State.SearchEnemy, new SearchEnemyBoidState(this,this,_fsm));
        _fsm.AddState(FSM.State.Retreat, new RetreatBoidState(this,this,_fsm));
        _life = new Life(this.gameObject,_maxLife, _slider);
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
            if (boid == null) continue;
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
    public void ApplyEnemySeparation(float range)
    {
        var enemies = BoidManager.instance.GetBoids.FindAll(b => b.typeBoid != typeBoid);
        AddForce(Separation(enemies, range) * enemySeparationWeight);
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
        if (enemyLeader != null)
        {
            if(FOV.InFieldOfView(enemyLeader.transform, this.transform, _viewRadius, _viewAngle))
            {
                AlertAllies();
                _alreadyAlerted = true;
                return true;
            }
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
            if (boid == null) continue;
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
            if (boid == null) continue;
            if (boid.typeBoid == this.typeBoid)
            {
                boid.ForceAttack();
            }
        }
        if (_leaderRef != null)
        {
            _leaderRef.ForceAttack();
        }
    }
    public Transform GetClosestVisibleEnemy()
    {
        var enemyLeader = LeaderManager.instance.GetLeader(_leaderRef);
        if (enemyLeader != null && FOV.InFieldOfView(enemyLeader.transform, this.transform, _viewRadius, _viewAngle))
            return enemyLeader.transform;
        var allBoids = BoidManager.instance.GetBoids;
        List<Boid> enemyBoids = allBoids.FindAll(b => b != null && b.typeBoid != this.typeBoid);
        Transform closest = null;
        float closestDist = Mathf.Infinity;
        foreach (var boid in enemyBoids)
        {
            if (FOV.InFieldOfView(boid.transform, this.transform, _viewRadius, _viewAngle))
            {
                float dist = Vector3.Distance(this.transform.position, boid.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = boid.transform;
                }
            }
        }
        return closest;
    }
    public void CheckHasNeighbors(ref bool _hasNeighbors)
    {
        _hasNeighbors = false;
        foreach (var boid in _neigboards)
        {
            if (boid == this) continue;
            if (boid == null) continue;
            float dist = Vector3.Distance(this.transform.position, boid.transform.position);
            if (dist < radiusSeparation)
            {
                _hasNeighbors = true;
                return;
            }
        }
    }
    public void CheckHasEnemyNeighbors(ref bool hasEnemyNearby, float range)
    {
        hasEnemyNearby = false; // Resetear antes de chequear
        var allBoids = BoidManager.instance.GetBoids;
        foreach (var boid in allBoids)
        {
            if (boid.typeBoid == this.typeBoid)continue;
            if (boid == null) continue;
            float dist = Vector3.Distance(this.transform.position, boid.transform.position);
            if (dist < range)
            {
                hasEnemyNearby = true;
                return; // Ya encontramos un enemigo cercano, no hace falta seguir
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
        ChangeMove(true);
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
        _fsm.RemoveState(FSM.State.Retreat);
        BoidManager.instance.RemoveBoid(this);
        GameManager.instance.UpdateAllNeighbors();
        _fsm = null;
        _life = null;
    }
    public Leader Leader { get => _leaderRef; }
    public Life Life { get => _life; }
    public List<Vector3> GetPath { get => _currentPath; }
    public float MinLifeToRetreat { get => _minLifeToRetreat; }
}