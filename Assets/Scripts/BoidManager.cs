using System.Collections.Generic;
using UnityEngine;
public class BoidManager : MonoBehaviour
{
    [SerializeField]private List<Boid> _allBoids = new List<Boid>();
    public static BoidManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        var boids = FindObjectsOfType<Boid>();
        foreach (var boid in boids)
        {
            RegisterBoid(boid);
        }
    }
    public void RegisterBoid(Boid boid)
    {
        if (!_allBoids.Contains(boid))
            _allBoids.Add(boid);
    }
    public void RemoveBoid(Boid boid)
    {
        if (_allBoids.Contains(boid))
            _allBoids.Remove(boid);
    }
    public List<Boid> GetBoids { get => _allBoids; }
}
