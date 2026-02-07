using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Agent))]
public class ObstacleAvoidance : MonoBehaviour
{
    private Agent _agent;
    private void Awake()
    {
        _agent = this.GetComponent<Agent>();
    }
}
