using System.Collections.Generic;
using UnityEngine;
public class Leader : Agent
{
    private FSM _fsm;
    [Header("LeaderConfig")]
    [SerializeField]private bool _isVioletLeader;
    [SerializeField]private float _maxLife;
    private Life _life;
    [Header("MoveState")]
    [SerializeField][Range(0,3)]private float _nearDistance;
    private List<Vector3> _mainPath = new List<Vector3>();
    [Header("FOV")]
    [SerializeField]private float _viewRadius;
    [SerializeField]private float _viewAngle;
    private void OnEnable()
    {
        LeaderManager.instance.Register(this);
        _fsm = new FSM();
        _fsm.AddState(FSM.State.Move, new MoveLeaderState(this.transform, _nearDistance,this,_mainPath,this,_fsm));
        _fsm.AddState(FSM.State.Idle, new IdleLeaderState(this,_mainPath, this, _fsm));
        _fsm.AddState(FSM.State.Attack, new AttackLeaderState(this));
        _life = new Life(_maxLife);
    }
    protected override void Start()
    {
        base.Start();
        _fsm.ChangeState(FSM.State.Idle);
    }
    protected override void Update()
    {
        DestinationSelection();
        _fsm?.OnUpdate();
        base.Update();
    }
    private void DestinationSelection()
    {
        var RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(RayCast, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Floor")))
        {
            if (!_isVioletLeader)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (LineOfSight.IsOnSight(this.transform.position, hitInfo.point))
                    {
                        GoDirectToTarget(hitInfo.point);
                    }
                    else
                    {
                        CalculatePathToTarget(hitInfo.point);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (LineOfSight.IsOnSight(this.transform.position, hitInfo.point))
                    {
                        GoDirectToTarget(hitInfo.point);
                    }
                    else
                    {
                        CalculatePathToTarget(hitInfo.point);
                    }
                }
            }
        }
    }
    private void GoDirectToTarget(Vector3 target)
    {
        _mainPath.Clear();
        _mainPath.Add(target);
    }
    private void CalculatePathToTarget(Vector3 target)
    {
        _mainPath.Clear();
        var path = PathFinding.CalculateTheta(this.transform.position, target);
        _mainPath.AddRange(path); // Cambio la lista no creo una nueva para no perder la referencia
    }
    public void RotateTo(Vector3 dir)
    {
        if(dir != Vector3.zero)
        {
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    public void ForceAttack()
    {
        _fsm.ChangeState(FSM.State.Attack);
    }
    public bool DetectEnemy()
    {
        var enemyLeader = LeaderManager.instance.GetLeader(this);
        if (enemyLeader != null && FOV.InFieldOfView(enemyLeader.transform, this.transform, _viewRadius, _viewAngle))
        {
            _fsm.ChangeState(FSM.State.Attack);
            AlertBoids();
            return true;
        }
        var allBoids = BoidManager.instance.GetBoids;
        List<Boid> enemyBoids;
        if (_isVioletLeader)
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
                _fsm.ChangeState(FSM.State.Attack);
                AlertBoids();
                return true;
            }
        }
        return false;
    }
    private void AlertBoids()
    {
        var allBoids = BoidManager.instance.GetBoids;
        foreach (var boid in allBoids)
        {
            if (boid.typeBoid == (_isVioletLeader ? TypeBoid.VioletTeam : TypeBoid.BlueTeam))
            {
                boid.ForceAttack();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusArrive);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nearDistance);
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
        _fsm = null;
        _life = null;
    }
    public bool IsVioletLeader { get => _isVioletLeader; }
}
