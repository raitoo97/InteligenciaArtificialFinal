using System.Collections.Generic;
using UnityEngine;
public class Node : MonoBehaviour
{
    [SerializeField]private List<Node> _neighbords = new List<Node>();
    public float cost;
    private void Awake()
    {
        NodeManager.RegisterNode(this);
    }
    private void Start()
    {
        NodeManager.CompleteNeighboards();
    }
    private void OnDestroy()
    {
        NodeManager.UnregisterNode(this);
    }
    public void AddNeighbord(Node node)
    {
        if(!_neighbords.Contains(node))
            _neighbords.Add(node);
    }
    public List<Node> Neighbords { get => _neighbords; }
}
