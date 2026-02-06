using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Boid> boids = new List<Boid>();
    [Range(0f, 1f)]public float weightSeparation;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        ConfigBoids();
    }
    private void ConfigBoids()
    {
        boids = BoidManager.instance.GetBoids;
        foreach(var boid in boids)
        {
            boid._neigboards = boids;
            boid.weightSeparation = weightSeparation;
        }
    }
}
