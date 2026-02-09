using System.Collections.Generic;
using UnityEngine;
public class LeaderManager : MonoBehaviour
{
    public static LeaderManager instance;
    [SerializeField]private List<Leader> _leaders = new List<Leader>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void Register(Leader leader)
    {
        if (!_leaders.Contains(leader))
            _leaders.Add(leader);
    }
    public void Remove(Leader leader)
    {
        if (_leaders.Contains(leader))
            _leaders.Remove(leader);
    }
    public Leader GetLeader(Leader leader)
    {
        if (leader.IsVioletLeader)
            return _leaders.Find(l => !l.IsVioletLeader);
        else
            return _leaders.Find(l => l.IsVioletLeader);
    }
}
