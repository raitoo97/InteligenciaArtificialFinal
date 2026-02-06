using System.Collections.Generic;
using UnityEngine;
public class Leader : Agent
{
    private FSM _fsm;
    [Header("LeaderConfig")]
    [SerializeField]private bool _isVioletLeader;
    [Header("MoveState")]
    [SerializeField][Range(0,3)]private float _nearDistance;
    private List<Vector3> _mainPath = new List<Vector3>();
    private void OnEnable()
    {
        _fsm = new FSM();
        _fsm.AddState(FSM.State.Move, new MoveLeaderState(this.transform, _nearDistance,_mainPath,this,_fsm));
        _fsm.AddState(FSM.State.Idle, new IdleLeaderState(_mainPath, this, _fsm));
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
                        Debug.Log("Esta en vision voy directo");
                        GoDirectToTarget(hitInfo.point);
                    }
                    else
                    {
                        Debug.Log(" NO Esta en vision calculo camino");
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
                        Debug.Log("Esta en vision voy directo");
                        GoDirectToTarget(hitInfo.point);
                    }
                    else
                    {
                        Debug.Log(" NO Esta en vision calculo camino");
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusArrive);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nearDistance);
    }
}
