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
        var blueBoids = boids.FindAll(b => b.typeBoid == TypeBoid.BlueTeam);
        var violetBoids = boids.FindAll(b => b.typeBoid == TypeBoid.VioletTeam);
        foreach (var boid in boids)
        {
            if (boid.typeBoid == TypeBoid.BlueTeam)
                boid._neigboards = blueBoids;
            else
                boid._neigboards = violetBoids;
            boid.weightSeparation = weightSeparation;
        }
    }
}
