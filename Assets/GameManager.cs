using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Boid> boids = new List<Boid>();
    [Range(0f, 2f)]public float weightSeparation;
    [Range(0f, 1f)] public float weightSeparationLeader;
    [Range(0f, 2f)] public float weightSeparationEnemy;
    public Text finishText;
    private bool gameEnded;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        Time.timeScale = 1f;
        gameEnded = false;
        finishText.gameObject.SetActive(false);
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
            boid.leaderSeparationWeight = weightSeparationLeader;
            boid.enemySeparationWeight = weightSeparationEnemy;
        }
    }
    private void Update()
    {
        CheckFinishCondition();
    }
    public void UpdateAllNeighbors()
    {
        var allBoids = BoidManager.instance.GetBoids;
        var blueBoids = allBoids.FindAll(b => b != null && b.typeBoid == TypeBoid.BlueTeam);
        var violetBoids = allBoids.FindAll(b => b != null && b.typeBoid == TypeBoid.VioletTeam);
        foreach (var boid in allBoids)
        {
            if (boid == null) continue;
            if (boid.typeBoid == TypeBoid.BlueTeam)
                boid._neigboards = new List<Boid>(blueBoids);
            else
                boid._neigboards = new List<Boid>(violetBoids);
        }
    }
    public void CheckFinishCondition()
    {
        boids = BoidManager.instance.GetBoids;
        var blueBoids = boids.FindAll(b => b.typeBoid == TypeBoid.BlueTeam);
        var violetBoids = boids.FindAll(b => b.typeBoid == TypeBoid.VioletTeam);
        var leader = LeaderManager.instance.Leaders;
        var blueLeader = leader.FindAll(x => !x.IsVioletLeader);
        var violetLeader = leader.FindAll(x => x.IsVioletLeader);
        bool violetWins = blueBoids.Count <= 0 && blueLeader.Count <= 0;
        bool blueWins = violetBoids.Count <= 0 && violetLeader.Count <= 0;
        if ((violetWins || blueWins) && !gameEnded)
        {
            gameEnded = true;
            finishText.gameObject.SetActive(true);
            Time.timeScale = 0;
            if (violetWins)
            {
                finishText.text = "Violet Team Wins";
                finishText.color = Color.magenta;
            }
            if (blueWins) 
            {
                finishText.text = "Blue Team Wins";
                finishText.color = Color.blue;
            }
        }
    }
}